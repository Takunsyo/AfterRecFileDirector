using System;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Data;
using RVMCore.GoogleWarpper;
using RVMCore.MasterView.ViewModel;
using System.Collections.Generic;
using System.Windows.Input;

namespace RVMCore.MasterView
{
    public class UploaderViewModel : ViewModelBase, IDisposable
    {
        //public:
        public string MainName { get; set; }="After record upload service GOOGLE DEMON.";
        public string NowProcressingContent { get; set; }

        public Color ProcessStateColor { get; set; } = Color.FromArgb(0xFF, 0x06, 0xB0, 0x25);
        public Brush ProcessStateBrush {
            get
            {
                return new SolidColorBrush( ProcessStateColor);
            }

        }
        public UploadFile SelectedItem { get; set; }
        private UploadFile mUpObj;
        public bool ProcessNowState { get; set; }
        public ProgressInfo ProcessNow { get; set; } = new ProgressInfo();
        public ProgressInfo ProcessGen { get; set; } = new ProgressInfo();
        public string ThreadControlName { get; set; }="Start";
        //private List<UploadFile> fileList = new List<UploadFile>();
        //public List<UploadFile> FileList { get {return fileList; } set {fileList = value; this.OnPropertyChanged("ThreadControlName"); } }

        public int MaxSpeed { get; set; }
        private bool isSpeedControl = false;
        public bool IsSpeedControl
        {
            get
            {
                return isSpeedControl;
            }
            set
            {
                if (value)
                {
                    if (MaxSpeed >= 256)
                    {
                        Service.MaxBytesPerSecond = (ulong)MaxSpeed * 1024;
                    }
                    else
                    {
                        this.MaxSpeed = 256;
                        Service.MaxBytesPerSecond = (ulong)MaxSpeed * 1024;
                    }
                }
                else
                {
                    Service.MaxBytesPerSecond = 0;
                }
                isSpeedControl = value;
            }
        }
        public ObservableCollection<UploadFile> FileList { get; set; }

        private PipeServer<RmtFile> midObject;

        private Database mDatabase;
        public bool IsWorking
        {
            get
            {
                if (mThread != null && mThread.IsAlive)
                {
                    return true;
                }
                else return false;
            }
        }

        private System.Threading.Timer DBTimer;

        private void DBOperator(object state)
        {
            //Check dbonline or not.
            if(this.mDatabase is null)
            {
                try
                {
                    this.mDatabase = new Database();
                }
                catch
                {
                    return;
                }
            }
            try
            {
                var locker = new object();
                BindingOperations.EnableCollectionSynchronization(this.FileList, locker);
                foreach (var item in this.mDatabase.LoadData())
                {
                    if (!FileList.Any(x => x.ID == item.ID)) this.Execute(() => FileList.Add(new UploadFile(item, mDatabase)));
                }
                BindingOperations.DisableCollectionSynchronization(this.FileList);
                FileList.ForEach(x => x.RefreshData());
                this.TotalLength = FileList.Sum(x => x.Length);
                this.Processed = FileList.Sum(x => x.IsOver ? x.Length : 0);
            }
            catch
            {
                return;
            }
        }

        public UploaderViewModel()
        {
            //Set a named pipe to accept commands from other instanes.
            midObject = new PipeServer<RmtFile>("RVMCoreUploader");
            midObject.PipeMessage += midObjectFileRecivedHdlr;
            if(!midObject.StartListen())MessageBox.Show("Unable to dig pip holes.", "Error",  MessageBoxButtons.OK);
            DBTimer = new System.Threading.Timer(this.DBOperator,null,1000, 180000);
            //Define uploading thread workload 
            this.UploadWorkload = new ThreadStart(() =>
            {
                do
                 {
                    ThreadControlName = "Pause";
                    this.mUpObj = null;
                     try
                     {
                         this.mUpObj = FileList.First(x => !x.IsOver);
                     }
                     catch(Exception ex )
                     {
                        ex.Message.InfoLognConsole();
                    }
                    if (this.mUpObj is null) break;
                    this.mUpObj.RefreshData();
                    string remot = this.mUpObj.RemotePath;
                    MainName = "[Uploading]" + this.mUpObj.FileName;
                    NowProcressingContent = this.mUpObj;
                    ProcessNow.SetValue(0, 0, "Initialize upload (MD5 File Checking...)");
                    ProcessNowState = true;
                    bool isSuccess = false;
                    if (!System.IO.File.Exists(GoogleDrive.GetUploadStatusPath(this.mUpObj)))
                    {   //if this is the first time this file is being upload.
                        if (!(bool)Service.RemoteFileExists(this.mUpObj, remot,false))
                        {
                            ProcessNowState = false;
                            if (this.mUpObj.IsChangeFather)
                            {
                                Google.Apis.Drive.v3.Data.File parent;
                                string nowParentName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(this.mUpObj.FullPath));
                                string remotePath = this.mUpObj.RemotePath.Replace(nowParentName, this.mUpObj.OldFatherPath);
                                if (!remotePath.IsNullOrEmptyOrWhiltSpace())
                                {
                                    parent = Service.GetGoogleFolderByPath(remotePath);
                                    Service.UpdateFile(parent.Id, nowParentName);
                                }
                            }
                            isSuccess = this.mUpObj.Upload(Service);
                        }
                        else
                        {
                            ProcessNowState = false;
                            isSuccess = true;
                        }
                    }
                    else
                    { //if this is a resume upload.
                        ProcessNowState = false;
                        isSuccess = this.mUpObj.Upload(Service);
                    }
                    if (UploadTokenSource?.Token.IsCancellationRequested ?? false) break;
                    Processed += this.mUpObj.Length;
                    ProcessNow.SetValue(0, 0, "Done.");
                    this.mUpObj.IsOver = isSuccess;
                } while (true);
                ThreadControlName = "Start";
            });
            this.mThread = new Thread(UploadWorkload);
            mThread.IsBackground = true;
            Service = new GoogleDrive();
            FileList = new ObservableCollection<UploadFile>();
            Service.UpdateProgressChanged += new UpdateProgress(mProcessHandller);
        }

        private void midObjectFileRecivedHdlr(object sender,RmtFile e)
        {
            if (e is null) return;
            try
            {
                var locker = new object();
                BindingOperations.EnableCollectionSynchronization(this.FileList, locker);
                var upfile = new UploadFile(e);
                this.Execute(() => this.FileList.Add(upfile)); //Add new file.
                this.Execute(() => FileList.ForEach(x => x.RefreshData())); // refresh all items.
                this.TotalLength = FileList.Sum(x => x.Length); //cal total size.
                this.Processed = FileList.Sum(x => x.IsOver ? x.Length : 0); // calic all uploaded.
                BindingOperations.DisableCollectionSynchronization(this.FileList);
                if (!this.IsWorking && SettingObj.Read().StartUploadWhenDataAvailable) this.StartOperation(null);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"File:[{e?.FullFilePath}] \nERR:[{ex.Message}]", 
                    "Record message failed to process!",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }


        //private:
        Thread mThread;
        ThreadStart UploadWorkload;
        CancellationTokenSource UploadTokenSource;
        public GoogleDrive Service { get; private set; } = null;

        private readonly Color PausedColor = Color.FromArgb(0xFF, 0xFF, 0xB9, 0x00);
        private readonly Color ProcesColor = Color.FromArgb(0xFF, 0x06, 0xB0, 0x25);

        private long TotalLength = 0;
        private long Processed = 0;
        //EventHandlling:
        private void mProcessHandller(long value, long maxmum, int y) 
        {
            if (value >= 0 && maxmum > 0)
            {
                if (value == 0)
                {
                    FileList.Single(x => x.FullPath == NowProcressingContent).UpdateData();
                }
                ProcessNowState = false;
                this.ProcessNow.SetValue(value, maxmum, (long)y);
                this.ProcessGen.SetValue(Processed + value, TotalLength);
            }
        }
        public ICommand OpenFileCommand => new CustomCommand(OpenFile);
        private void OpenFile(object sender)
        {
            using (var mdlg = new OpenFileDialog())
            {
                mdlg.CheckFileExists = true;
                mdlg.Filter = "Transport Stream(*.ts;*.m2ts)|*.ts;*.m2ts|Meta Data(*.meta;*.xml)|*.meta;*.xml|All file(*.*)|*.*";
                //mdlg.InitialDirectory = @"E:\1[アニメ類]\";
                mdlg.Multiselect = true;
                if (mdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string i in mdlg.FileNames)
                    {
                        if (!FileList.Any(x => x == i)) FileList.Add(i);
                    }
                }
                else return;
            }
            FileList.ForEach(x => x.RefreshData());
            this.TotalLength = FileList.Sum(x => x.Length);
            this.Processed = FileList.Sum(x => x.IsOver ? x.Length : 0);
        }
        public ICommand StartOperationCommand => new CustomCommand(StartOperation);
        private void StartOperation(object sender)
        {
            if (mThread == null || !mThread.IsAlive)
            {
                UploadTokenSource?.Dispose();
                mThread = new Thread(UploadWorkload);
                UploadTokenSource = new CancellationTokenSource();
                mThread.Start();
                return;
            }
            if (this.mUpObj.ThreadState.HasFlag(ThreadState.Unstarted)) return;
            if (this.mUpObj.ThreadState.HasFlag(ThreadState.Suspended)||
                this.mUpObj.ThreadState.HasFlag(ThreadState.SuspendRequested))
            {
                //mThread.Resume();
                this.mUpObj.Resume();
                ProcessStateColor = this.ProcesColor;
                ThreadControlName = "Pause";
            }
            else
            {
                //mThread.Suspend();
                this.mUpObj.Pause();
                ProcessStateColor = this.PausedColor;
                ThreadControlName = "Resume";
            }
        }
        public ICommand RemoveItemCommand => new CustomCommand(RemoveItem);
        private void RemoveItem(object sender)
        {
            if (this.SelectedItem == null) return;
            try
            {
                string info = "\n File:" + System.IO.Path.GetFileName(this.SelectedItem.FullPath) + "\n" +
                                "Remot:" + this.SelectedItem.RemotePath + "\n" +
                                " Size:" + this.SelectedItem.Size;
                if (this.SelectedItem.IsUploading)
                {
                    if(MessageBox.Show("Current object is uploading... \n Do you want to stop current upload process?"+info,
                                        "Removing object from list.",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        SelectedItem.Abort();
                        this.FileList.Remove(this.SelectedItem);
                        this.mDatabase.SetUploadStatus(this.SelectedItem.ID, true, false);
                    }
                }
                else
                {
                    if (this.SelectedItem.IsOver || 
                        MessageBox.Show("Do you want to remove this file?" + info, 
                                        "Removing object from list.", 
                                        MessageBoxButtons.YesNo, 
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    { 
                        this.FileList.Remove(this.SelectedItem);
                        this.mDatabase.SetUploadStatus(this.SelectedItem.ID, true, false);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error");
            }
            this.TotalLength = FileList.Sum(x => x.Length);
            this.Processed = FileList.Sum(x => x.IsOver ? x.Length : 0);
        }


        public ICommand ResetThreadsCommand => new CustomCommand(ResetThreads);
        private void ResetThreads(object sender)
        {
            if (mThread != null && mThread.IsAlive)
            {
                if (this.mUpObj.IsUploading)
                    this.mUpObj.Abort();
                //if (mThread.IsAlive) mThread.Abort();
                UploadTokenSource?.Cancel();
                mThread.Join();
                UploadTokenSource?.Dispose();
                mThread = new Thread(UploadWorkload);
                UploadTokenSource = new CancellationTokenSource();
                mThread.Start();
                ProcessStateColor = this.ProcesColor;
                ThreadControlName = "Pause";
                return;
            }
        }

        public ICommand UpItemCommand => new CustomCommand(UpItem);

        private void UpItem(object sender)
        {
            if (this.SelectedItem == null) return;
            int index = this.FileList.IndexOf(this.SelectedItem);
            if (index <= 0) return;
            this.FileList.Move(index, index - 1);
        }

        public ICommand DownItemCommand => new CustomCommand(DownItem);
        private void DownItem(object sender)
        {
            if (this.SelectedItem == null) return;
            int index = this.FileList.IndexOf(this.SelectedItem);
            if (index < 0 || index >= (this.FileList.Count-1)) return;
            this.FileList.Move(index, index + 1);
        }

        #region"IDispose"
        public void Dispose()
        {
            if (mThread!=null && mThread.IsAlive)
            {
                mThread.Abort();
            }
            DBTimer.Dispose();
            ((IDisposable)Service).Dispose();
            mDatabase.Dispose();
        }
        #endregion
    }
}
