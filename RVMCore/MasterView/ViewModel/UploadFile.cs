using RVMCore.GoogleWarpper;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RVMCore.MasterView.ViewModel
{

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
        public string FileName => System.IO.Path.GetFileName(this.FullPath);
        public string FullPath { get; }
        public bool IsUploading => System.IO.File.Exists(GoogleDrive.GetUploadStatusPath(this.FullPath));

        public bool IsChangeFather { get; } = false;
        public string OldFatherPath { get; } = null;
        public bool GoForUpload { get; private set; } = true;
        public bool IsOver { get; set; }
        public string RemotePath => System.IO.Path.GetDirectoryName(this.FullPath).Replace(System.IO.Path.GetPathRoot(this.FullPath), @"\EPGRecords\");

        public string Size => getSizeString((ulong)Length);

        public long Length { get; private set; }
        public UploadFile(string filePath)
        {
            FullPath = filePath;
            IsOver = false;
            var fileinfo = new System.IO.FileInfo(filePath);
            this.Length = fileinfo.Length;
        }
        public UploadFile(RmtFile file)
        {
            FullPath = file.FullFilePath;
            this.IsChangeFather = file.IsFatherUpdate;
            this.OldFatherPath = file.OldFatherName;
            this.GoForUpload = file.ProcessAnyway;
            IsOver = false;
            var fileinfo = new System.IO.FileInfo(file.FullFilePath);
            this.Length = fileinfo.Length;
        }

        public void RefreshData()
        {
            if (!System.IO.File.Exists(this.FullPath)) return;
            var fileinfo = new System.IO.FileInfo(this.FullPath);
            this.Length = fileinfo.Length;
        }
        public void UpdateData()
        {
            this.NotifyPropertyChanged("ShowName");
        }
        public bool UploadNow { get; set; } = true;

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
            if (!(mWork is null) && mWork.IsAlive)
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

        public ThreadState ThreadState
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
        public static implicit operator UploadFile(RmtFile input)
        {
            return new UploadFile(input);
        }

        public static implicit operator RmtFile(UploadFile input)
        {
            return new RmtFile(input.FullPath, input.IsChangeFather, input.OldFatherPath, input.GoForUpload);
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
