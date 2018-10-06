using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.Google
{
    interface IGoogleDriveObject
    {
        string ID { get;}
        string Name { get; }
        string MIMEType { get; }
        IGoogleDriveObject Parent { get; }
        void DeleteObject();
        void DownloadObject();
        void DownloadObject(string filePath);
        void RelocateObject(IList<string> parents);
    }
}
