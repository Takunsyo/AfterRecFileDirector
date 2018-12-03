using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Threading;
using RVMCore.Forms;

namespace RVMCore.GoogleWarpper
{
    class UploaderViewModel : ViewModelBase, IDisposable
    {
        //public:
        private string mainname = "After record upload service GOOGLE DEMON.";
        public string MainName { get { return this.mainname; } set { this.SetProperty(ref this.mainname, value); } }
        private string nowProcressingContent;
        public string NowProcressingContent {
            get
            {
                return nowProcressingContent;
            }
            set
            {
                nowProcressingContent = value;
                this.OnPropertyChanged("NowProcressingContent");
            }
        }

        private Color processStateColor = Color.FromArgb(0xFF, 0x06, 0xB0, 0x25);
        public Color ProcessStateColor
        {
            get
            {
                return processStateColor;
            }
            set
            {
                processStateColor = value;
                this.OnPropertyChanged("ProcessStateColor");
                this.OnPropertyChanged("ProcessStateBrush");
            }
        }
        public Brush ProcessStateBrush {
            get
            {
                return new SolidColorBrush( processStateColor);
            }

        }
        private UploadFile _selectedItem;
        public UploadFile SelectedItem {
            get
            {
                return _selectedItem;
            }
            set
            {
                this.SetProperty(ref _selectedItem, value);
            }
        }
        private UploadFile mUpObj;
        private bool processNowState = false;
        public bool ProcessNowState { get { return processNowState; } set { processNowState = value; this.OnPropertyChanged("ProcessNowState");}}
        private ProgressInfo processNow = new ProgressInfo();
        public ProgressInfo ProcessNow { get { return processNow; } set { processNow = value; this.OnPropertyChanged("ProcessNow"); } }
        private ProgressInfo processGen = new ProgressInfo();
        public ProgressInfo ProcessGen { get { return processGen; } set { processGen = value; this.OnPropertyChanged("ProcessGen"); } }
        private string threadControlName = "Start";
        public string ThreadControlName { get { return threadControlName; } set { threadControlName = value; this.OnPropertyChanged("ThreadControlName"); } }
        //private List<UploadFile> fileList = new List<UploadFile>();
        //public List<UploadFile> FileList { get {return fileList; } set {fileList = value; this.OnPropertyChanged("ThreadControlName"); } }

        private int maxSpeed = 0;
        public int MaxSpeed
        {
            get
            {
                return maxSpeed;
            }
            set
            {
                maxSpeed = value;
                this.OnPropertyChanged("MaxSpeed");
            }
        }
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
                    if (maxSpeed >= 256)
                    {
                        Service.MaxBytesPerSecond = (ulong)maxSpeed * 1024;
                    }
                    else
                    {
                        this.MaxSpeed = 256;
                        Service.MaxBytesPerSecond = (ulong)maxSpeed * 1024;
                    }
                }
                else
                {
                    Service.MaxBytesPerSecond = 0;
                }
                isSpeedControl = value;
                this.OnPropertyChanged("IsSpeedControl");

            }
        }
        public ObservableCollection<UploadFile> FileList { get; set; }

        private PipeServer<RmtFile> midObject;

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
        public UploaderViewModel()
        {
            //Set a named pipe to accept commands from other instanes.
            midObject = new PipeServer<RmtFile>("RVMCoreUploader");
            midObject.PipeMessage += midObjectFileRecivedHdlr;
            if(!midObject.StartListen())MessageBox.Show("Unable to dig pip holes.", "Error",  MessageBoxButtons.OK);
            //Define uploading thread workload
            this.UploadWorkload = new System.Threading.ThreadStart(() =>
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
                    string remot = this.mUpObj.RemotePath;
                    MainName = "[Uploading]" + this.mUpObj.FileName;
                    NowProcressingContent = this.mUpObj;
                    ProcessNow.SetValue(0, 0, "Initialize upload (MD5 File Checking...)");
                    ProcessNowState = true;
                    bool isSuccess = false;
                    if (!System.IO.File.Exists(RVMCore.GoogleWarpper.GoogleDrive.GetUploadStatusPath(this.mUpObj)))
                    {   
                        if (!(bool)Service.RemoteFileExists(this.mUpObj, remot,false))
                        {
                            ProcessNowState = false;
                            isSuccess = this.mUpObj.Upload(Service);
                        }
                        else
                        {
                            ProcessNowState = false;
                            isSuccess = true;
                        }
                    }
                    else
                    {
                        ProcessNowState = false;
                        isSuccess = this.mUpObj.Upload(Service);
                    }

                    Processed += this.mUpObj.Length;
                    ProcessNow.SetValue(0, 0, "Done.");
                    this.mUpObj.IsOver = isSuccess;
                } while (true);
                ThreadControlName = "Start";
            });
            this.mThread = new System.Threading.Thread(UploadWorkload);
            mThread.IsBackground = true;
            Service = new GoogleWarpper.GoogleDrive();
            FileList = new ObservableCollection<UploadFile>();
            Service.UpdateProgressChanged += new GoogleWarpper.UpdateProgress(mProcessHandller);
        }

        private void midObjectFileRecivedHdlr(RmtFile e)
        {
            this.FileList.Add(e.FullFilePath);
        } 


        //private:
        System.Threading.Thread mThread;
        System.Threading.ThreadStart UploadWorkload;
        GoogleWarpper.GoogleDrive Service = null;

        private readonly Color PausedColor = Color.FromArgb(0xFF, 0xFF, 0xB9, 0x00);
        private readonly Color ProcesColor = Color.FromArgb(0xFF, 0x06, 0xB0, 0x25);

        private long TotalLength = 0;
        private long Processed = 0;
        //EventHandlling:
        private void mProcessHandller(long value, long maxmum, int y) 
        {
            if (value >= 0 && maxmum > 0)
            {
                if (value == 0) {
                    FileList.Single(x => x.FullPath == NowProcressingContent).UpdateData();
                }
                ProcessNowState = false;
                this.ProcessNow.SetValue(value, maxmum, (long)y);
                this.processGen.SetValue(Processed + value, TotalLength);
            }
        }
        public void Open_Click(object sender, EventArgs e)
        {
            using (var mdlg = new System.Windows.Forms.OpenFileDialog())
            {
                mdlg.CheckFileExists = true;
                mdlg.Filter = "Transport Stream(*.ts;*.m2ts)|*.ts;*.m2ts|Meta Data(*.meta;*.xml)|*.meta;*.xml|All file(*.*)|*.*";
                mdlg.InitialDirectory = @"E:\1[アニメ類]\";
                mdlg.Multiselect = true;
                if (mdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string i in mdlg.FileNames)
                    {
                        if (!FileList.Any(x => x == i)) FileList.Add(i);
                    }
                }
                else return;
            }
            this.TotalLength = FileList.Sum(x => x.Length);
            this.Processed = FileList.Sum(x => x.IsOver ? x.Length : 0);
        }

        public void Start_Click(object sender, EventArgs e)
        {
            if (mThread == null || !mThread.IsAlive) { mThread = new System.Threading.Thread(UploadWorkload); mThread.Start(); return; }
            if (this.mUpObj.ThreadState.HasFlag(System.Threading.ThreadState.Unstarted)) return;
            if (this.mUpObj.ThreadState.HasFlag(System.Threading.ThreadState.Suspended)|| this.mUpObj.ThreadState.HasFlag(System.Threading.ThreadState.SuspendRequested))
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

        public void RemoveItem(object sender, EventArgs e)
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
                    }
                }
                else
                {
                    if (this.SelectedItem.IsOver || 
                        MessageBox.Show("Do you want to remove this file?" + info, 
                                        "Removing object from list.", 
                                        MessageBoxButtons.YesNo, 
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
                        this.FileList.Remove(this.SelectedItem);
                }
            }
            catch
            {
                MessageBox.Show("Error");
            }
            this.TotalLength = FileList.Sum(x => x.Length);
            this.Processed = FileList.Sum(x => x.IsOver ? x.Length : 0);
        }

        public void ResetThreads()
        {
            if (mThread != null && mThread.IsAlive)
            {
                if (mThread.IsAlive) mThread.Abort();
                if (this.mUpObj.IsUploading)
                    this.mUpObj.Abort();
                Thread.Sleep(1000);
                mThread = new Thread(UploadWorkload);
                mThread.Start();
                return;
            }
        }

        public void UpItem(object sender, EventArgs e)
        {
            if (this.SelectedItem == null) return;
            int index = this.FileList.IndexOf(this.SelectedItem);
            if (index <= 0) return;
            this.FileList.Move(index, index - 1);
        }

        public void DownItem(object sender, EventArgs e)
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
            ((IDisposable)Service).Dispose();
        }
        #endregion
    }

    public class ProgressInfo : ViewModelBase
    {
        public string Text
        {
            get
            {
                if (max == val) return "" + Extra;
                return "[" + getSizeString(val) + "/" + getSizeString(max) + "]" + Extra;
            }
        }
        private ulong max;
        public int Max
        {
            get
            {
                //if(max > int.MaxValue)
                //{
                //    return (int)(max / 2);
                //}
                return (int)(max / 256);
            }
            set { }//max = (ulong)value; }
        }
        private ulong val;
        public int Val
        {
            get
            {
                //if(max > int.MaxValue)
                //{
                //    return (int)(val / 2);
                //}
                return (int)(val / 256);
            }
            set { }// val = (ulong)value; }
        }
        public string Extra { get; set; }
        private string getSizeString(ulong size)
        {
            string tmp = "";
            if (size > 1024 * 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024 / 1024).ToString("F2") + " Gb";
            }
            else if (size > 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024).ToString("F2") + " Mb";
            }
            else if (size > 1024)
            {
                tmp = ((float)size / 1024).ToString("F2") + " Kb";
            }
            else
            {
                tmp = (size).ToString() + " Byte";
            }
            return tmp;
        }
        public void SetValue(int val, string extra)
        {
            this.val = (ulong)val;
            this.Extra = extra;
            this.OnPropertyChanged("Extra");
            this.OnPropertyChanged("Val");
            this.OnPropertyChanged("Text");
        }
        public void SetValue(int val, int max, string extra)
        {
            this.val = (ulong)val;
            this.max = (ulong)max;
            this.Extra = extra;
            this.OnPropertyChanged("Val");
            this.OnPropertyChanged("Max");
            this.OnPropertyChanged("Extra");
            this.OnPropertyChanged("Text");
        }
        public void SetValue(long val, string extra)
        {
            this.val = (ulong)val;
            this.Extra = extra;
            this.OnPropertyChanged("Val");
            this.OnPropertyChanged("Extra");
            this.OnPropertyChanged("Text");
        }
        public void SetValue(long val, long max, string extra)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.Extra = extra;
            this.OnPropertyChanged("Max");
            this.OnPropertyChanged("Val");
            this.OnPropertyChanged("Extra");
            this.OnPropertyChanged("Text");
        }
        public void SetValue(long val, long max, long extra)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.Extra = getSizeString((ulong)extra) + "//s";
            this.OnPropertyChanged("Extra");
            this.OnPropertyChanged("Text");
            this.OnPropertyChanged("Max");
            this.OnPropertyChanged("Val");
        }
        public void SetValue(int val, int max, int extra)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.Extra = getSizeString((ulong)extra) + "//s";
            this.OnPropertyChanged("Extra");
            this.OnPropertyChanged("Text");
            this.OnPropertyChanged("Max");
            this.OnPropertyChanged("Val");
        }
        public void SetValue(int val, int max)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.OnPropertyChanged("Max");
            this.OnPropertyChanged("Text");
            this.OnPropertyChanged("Val");
        }
        public void SetValue(long val, long max)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.OnPropertyChanged("Max");
            this.OnPropertyChanged("Text");
            this.OnPropertyChanged("Val");
        }
        public ProgressInfo()
        {
            this.val = 0;
            this.max = 100;
            this.Extra = "";
        }
        
    }

    public class UploadFile : ViewModelBase
    {
        public string ShowName
        {
            get
            {
                if (this.IsOver) return "==" + this.FileName;
                else if (this.IsUploading) return "->" + this.FileName;
                else return "--" + this.FileName;
            }
        }
        public string FileName { get { return System.IO.Path.GetFileName(this.FullPath); } }
        public string FullPath { get; }
        public bool IsUploading { get { return System.IO.File.Exists(RVMCore.GoogleWarpper.GoogleDrive.GetUploadStatusPath(this.FullPath)); } }
        private bool _IsOVer;
        public bool IsOver
        {
            get
            {
                return _IsOVer;
            }
            set
            {
                this._IsOVer = value;
                this.NotifyPropertyChanged("ShowName");
            }
        }
        public string RemotePath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(this.FullPath).Replace(System.IO.Path.GetPathRoot(this.FullPath), @"\EPGRecords\");
            }
        }
        public string Size
        {
            get
            {
                return getSizeString((ulong)Length);
            }
        }
        public long Length { get; private set; }
        public UploadFile(string filePath)
        {
            FullPath = filePath;
            IsOver = false;
            var fileinfo = new System.IO.FileInfo(filePath);
            this.Length = fileinfo.Length;
        }
        public void UpdateData()
        {
            this.NotifyPropertyChanged("ShowName");
        }
        private bool _uploadNow = true;
        public bool UploadNow { get
            {
                return _uploadNow;
            }
            set
            {
                SetProperty(ref _uploadNow,value);
            }
        }

        public static IEnumerable<UploadFile> UploadFiles(IEnumerable<string> filePathes)
        {
            var tmp = new List<UploadFile>();
            foreach (string i in filePathes)
            {
                tmp.Add(new UploadFile(i));
            }
            return tmp;
        }

        private Thread mWork = null;
        public bool Upload(GoogleWarpper.GoogleDrive googleDrive)
        {
            bool result = false;
            var workLoad = new ThreadStart(() =>
            {
                result = googleDrive.UploadResumable(this.FullPath, this.RemotePath) != null;
            });
            mWork = new Thread(workLoad);
            mWork.IsBackground = true;
            mWork.Start();
            try
            {
                mWork.Join(Timeout.Infinite);
            }
            finally
            {
                mWork = null;
                GC.Collect();
            }
            return result;
        }

        public void Abort()
        {
            if(!(mWork is null) && mWork.IsAlive)
            {
                mWork.Abort();
            }
        }

        public void Pause()
        {
            if (!(mWork is null) && mWork.IsAlive)
            {
                mWork.Suspend();
            }
        }

        public void Resume()
        {
            if (!(mWork is null) && mWork.IsAlive)
            {
                mWork.Resume();
            }
        }

        public System.Threading.ThreadState ThreadState
        {
            get
            {
                if (mWork is null) return System.Threading.ThreadState.Unstarted;
                return mWork.ThreadState;
            }
        }


        public static implicit operator UploadFile(string input)
        {
            return new UploadFile(input);
        }

        public static implicit operator string(UploadFile input)
        {
            return input.FullPath;
        }

        public static bool operator ==(UploadFile obj1, UploadFile obj2)
        {
            if (obj1 is null || obj2 is null) return false;
            return obj1.FullPath == obj2.FullPath;
        }
        public static bool operator !=(UploadFile obj1, UploadFile obj2)
        {
            if (obj1 is null || obj2 is null) return true;
            return obj1.FullPath != obj2.FullPath;
        }

        public override int GetHashCode()
        {
            return this.FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is UploadFile)
            {
                return this == ((UploadFile)obj);
            }
            else
            {
                return false;
            }
        }

        private string getSizeString(ulong size)
        {
            string tmp = "";
            if (size > 1024 * 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024 / 1024).ToString("F2") + " Gb";
            }
            else if (size > 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024).ToString("F2") + " Mb";
            }
            else if (size > 1024)
            {
                tmp = ((float)size / 1024).ToString("F2") + " Kb";
            }
            else
            {
                tmp = (size).ToString() + " Byte";
            }
            return tmp;
        }
    }

}
