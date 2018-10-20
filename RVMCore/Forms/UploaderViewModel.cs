using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace RVMCore.Forms
{
    class UploaderViewModel : INotifyPropertyChanged, IDisposable
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

        public class UploadFile : INotifyPropertyChanged
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
            private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public static IEnumerable<UploadFile> UploadFiles(IEnumerable<string> filePathes)
            {
                var tmp = new List<UploadFile>();
                foreach (string i in filePathes)
                {
                    tmp.Add(new UploadFile(i));
                }
                return tmp;
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
                if (obj is null) return false;
                return this == ((UploadFile)obj);
            }
        }
        public class ProgressInfo : INotifyPropertyChanged
        {
            public string Text {
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
                    return (int)(max /256);
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
                    return (int)(val/256);
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
            public void SetValue(int val, int max ,string extra)
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
            public event PropertyChangedEventHandler PropertyChanged;

            protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
            {
                if (!EqualityComparer<T>.Default.Equals(field, newValue))
                {
                    field = newValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    return true;
                }
                return false;
            }
            public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        public UploaderViewModel()
        {
            this.UploadWorkload = new System.Threading.ThreadStart(() =>
             {
                 do
                 {
                     ThreadControlName = "Pause";
                     UploadFile local = null;
                     try
                     {
                         local = FileList.First(x => !x.IsOver);
                     }
                     catch
                     {
                         return;
                     }
                    //int index = FileList.FindIndex(x => x == local);
                    if (local == null) return;
                     string remot = local.RemotePath;
                     MainName = "[Uploading]" + local.FileName;
                     NowProcressingContent = local;
                     ProcessNow.SetValue(0, 0, "Initialize upload (MD5 File Checking...)");
                     ProcessNowState = true;
                     bool isSuccess = false;
                     if (!System.IO.File.Exists(RVMCore.GoogleWarpper.GoogleDrive.GetUploadStatusPath(local)))
                     {
                         if (!Service.RemoteFileExists(local, remot))
                         {
                             ProcessNowState = false;
                            //FileList[index] = local;
                            isSuccess = Service.UploadResumable(local, remot) != null;
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
                         isSuccess = Service.UploadResumable(local, remot) != null;
                     }
                    //button1.Invoke(new Action(() => { button1.Enabled = true; }));

                    ProcessNow.SetValue(0, 0, "Done.");
                     ThreadControlName = "Start";
                    //this.progressBar1.Style = ProgressBarStyle.Blocks;
                    local.IsOver = isSuccess;
                    //FileList[index] = local;
                } while (true);
             });
            this.mThread = new System.Threading.Thread(UploadWorkload);
            mThread.IsBackground = true;
            Service = new GoogleWarpper.GoogleDrive();
            FileList = new ObservableCollection<UploadFile>();
            Service.UpdateProgressChanged += new GoogleWarpper.UpdateProgress(mProcessHandller);
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
                if (value==0)FileList.Single(x=>x.FullPath ==NowProcressingContent).UpdateData();
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
                mdlg.Filter = "TransPortStream|*.ts";
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
            if (mThread.ThreadState == System.Threading.ThreadState.Suspended || mThread.ThreadState == System.Threading.ThreadState.SuspendRequested)
            {
                mThread.Resume();
                ProcessStateColor = this.ProcesColor;
                ThreadControlName = "Pause";
            }
            else
            {
                mThread.Suspend();
                ProcessStateColor = this.PausedColor;
                ThreadControlName = "Resume";
            }
        }

        #region"INotify"
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
}
