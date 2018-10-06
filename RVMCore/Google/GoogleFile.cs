using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.Google
{
    class GoogleFile : IGoogleDriveObject
    {
        private readonly string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private File mBaseFile;

        private DriveService mService;

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string MIMEType { get; private set; }

        public IGoogleDriveObject Parent { get; private set; }

        public delegate void FileProcessStatus(object sender,IDownloadProgress progress);

        public event FileProcessStatus OnFileStatusReviced;

        public void DeleteObject()
        {
            this.mService.Files.Delete(this.ID);
        }

        public void DownloadObject()
        {
            DownloadObject(System.IO.Path.Combine(DefaultPath, this.Name));
        }

        public void DownloadObject(string filePath)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(DefaultPath, this.Name)))
            {
                throw new InvalidOperationException("File exists!");
            }
            var mReq = mService.Files.Get(this.ID);
            using (var stream = new System.IO.FileStream(filePath,
                                                  System.IO.FileMode.Create,
                                                  System.IO.FileAccess.Write))
            {
                mReq.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
                { this.OnFileStatusReviced.Invoke(this, progress); };
                mReq.Download(stream);
            }
        }

        public void RelocateObject(IList<string> parents)
        {
            this.mBaseFile.Parents = parents;
            this.mService.Files.Update(this.mBaseFile, this.ID);
        }
    }
}
