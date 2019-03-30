using RVMCore.GoogleWarpper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

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
        public string FullPath { get; private set; }

        public bool IsUploading => CheckUploading();
        public string Name { get; set; } = null;
        public string ID { get; set; } = null;
        public bool IsChangeFather { get; } = false;
        public string OldFatherPath { get; } = null;
        public bool GoForUpload { get; private set; } = true;
        public bool IsOver { get; set; }
        public string RemotePath => System.IO.Path.GetDirectoryName(this.FullPath).Replace(System.IO.Path.GetPathRoot(this.FullPath), @"\EPGRecords\");

        public void UpdateFile(string fullPath)
        {
            this.FullPath = fullPath;
        }

        private Database mDatabase = null;
        public string Size => getSizeString((ulong)Length);

        private bool CheckUploading()
        {
            if (this.ID.IsNullOrEmptyOrWhiltSpace() || this.mDatabase is null)
            {
                return System.IO.File.Exists(GoogleDrive.GetUploadStatusPath(this.FullPath));
            }
            else
            {
                var rmtStatus = this.mDatabase.GetFileUploadStatus(this.ID, out var uid) ?? false;
                if (rmtStatus) return !rmtStatus;
                if (!(uid?.IsNullOrEmptyOrWhiltSpace() ?? true)) //if uid is not null
                {
                    if (uid.Length > 70) return true;
                    else return false;
                }
                return false;
            }
        }

        public long Length { get; private set; }
        public UploadFile(string filePath)
        {
            FullPath = filePath;
            IsOver = false;
            if (!System.IO.File.Exists(filePath))
                throw new ArgumentException($"File doesn't exists : [{filePath}]");
            var fileinfo = new System.IO.FileInfo(filePath);
            this.Length = fileinfo.Length;
        }

        public UploadFile(RmtFile file, Database database)
        {
            if (file is null || file.FullFilePath.IsNullOrEmptyOrWhiltSpace())
                throw new ArgumentException("Argument 'file' is null, or it doesn't contain any usable path.");
            if (!System.IO.File.Exists(file.FullFilePath))
                throw new ArgumentException($"File doesn't exists : [{file.FullFilePath}]");
            FullPath = file.FullFilePath;
            this.IsChangeFather = file.IsFatherUpdate;
            this.OldFatherPath = file.OldFatherName;
            this.GoForUpload = file.ProcessAnyway;
            IsOver = false;
            var fileinfo = new System.IO.FileInfo(file.FullFilePath);
            this.Length = fileinfo.Length;
            this.ID = file.ID;
            this.mDatabase = database;
        }

        public UploadFile(RmtFile file)
        {
            if (file is null || file.FullFilePath.IsNullOrEmptyOrWhiltSpace())
                throw new ArgumentException("Argument 'file' is null, or it doesn't contain any usable path.");
            if (!System.IO.File.Exists(file.FullFilePath))
                throw new ArgumentException($"File doesn't exists : [{file.FullFilePath}]");
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

        private CancellationTokenSource tokenSource;
        private ManualResetEvent manualReset;
        private ManualResetEvent finIndicator;
        private bool IsPaused => !(manualReset?.WaitOne(0) ?? true);
        public bool IsAlive { get; private set; } = false;
        public bool Upload(GoogleDrive googleDrive)
        {
            bool result = false;
            tokenSource = new CancellationTokenSource();
            manualReset = new ManualResetEvent(true);
            finIndicator = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(async x =>
            {
                this.IsAlive = true;
                try
                {
                    if (this.ID.IsNullOrEmptyOrWhiltSpace() || this.mDatabase is null)
                        result = await googleDrive.UploadResumableAsync(this.FullPath, this.RemotePath, tokenSource.Token, this.manualReset) != null;
                    else
                    {
                        string uri = this.mDatabase.GetFileUploadID(this.ID);
                        string getID(bool force)
                        {
                            if (string.IsNullOrWhiteSpace(uri) || force)
                            {
                                uri = googleDrive.GenerateUploadID(this.FullPath, this.RemotePath);
                                this.mDatabase.SetUploadStatus(this.ID, uri);
                            }
                            return uri;
                        }
                        var id = await googleDrive.UploadResumableAsync(this.FullPath, getID, tokenSource.Token, this.manualReset);
                        if (!id.IsNullOrEmptyOrWhiltSpace())
                        {
                            this.mDatabase.SetUploadStatus(this.ID, id, true);
                            result = true;
                        }
                        else
                            result = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ErrorLognConsole();
                    return;
                }
                finally
                {
                    this.IsAlive = false;
                    finIndicator.Set();
                }
            });
            try
            {
                finIndicator.WaitOne();
            }
            finally
            {
                tokenSource.Dispose();
                manualReset.Dispose();
                GC.Collect();
            }
            return result;
        }

        public void Abort()
        {
            this.tokenSource?.Cancel();
            if (this.IsPaused)
            {
                this.Resume(); //If paused make it alive again to cancel it.
            }
            finIndicator?.WaitOne(2500);
        }

        public void Pause() =>
            this.manualReset?.Reset();

        public void Resume() =>
                this.manualReset?.Set();

        public ThreadState ThreadState
        {
            get
            {
                if (!this.IsAlive) return ThreadState.Unstarted;
                if (this.IsPaused) return ThreadState.Suspended;
                else return ThreadState.Running;
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
            if (input is null) return null;
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

    [ValueConversion(typeof(double), typeof(double),ParameterType = typeof(double))]
    public class ListWidthNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(double) )
                throw new InvalidOperationException("The target must be a number");

            return (double)value - double.Parse((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a number");

            return (double)value + double.Parse((string)parameter);
        }
    }
}
