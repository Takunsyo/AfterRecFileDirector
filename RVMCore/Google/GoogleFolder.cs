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
    class GoogleFolder : IGoogleDriveObject
    {
        private readonly string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private File mBaseFile;

        private DriveService mService;

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string MIMEType { get; private set; }

        public IGoogleDriveObject Parent { get; private set; }

        public IList<IGoogleDriveObject> Childs { get; private set; }
        
        public delegate void FileProcessStatus(object sender, IDownloadProgress progress);

        public event FileProcessStatus OnFileStatusReviced;

        public void DeleteObject()
        {
            if(this.Childs==null && this.Childs.Count > 0)
            {
                foreach(IGoogleDriveObject obj in this.Childs)
                {
                    obj.DeleteObject();
                }
            }
            this.mService.Files.Delete(this.ID);
        }

        public void DownloadObject()
        {

        }

        public void DownloadObject(string filePath)
        {
            string baseFolderPath = System.IO.Path.Combine(DefaultPath, this.Name);
            if (System.IO.File.Exists(baseFolderPath))
            {
                // in case this is a folder, may need another way to process this kind of thing.
                throw new InvalidOperationException("File exists!");
            }
            else
            {
                foreach(IGoogleDriveObject obj in this.Childs)
                {
                    if (obj.MIMEType == "")
                    {
                        obj.DownloadObject(baseFolderPath);
                    }
                    else
                    {
                        obj.DownloadObject();
                    }
                }
            }
        }

        public void RelocateObject(IList<string> parents)
        {
            throw new NotImplementedException();
        }
    }
}
