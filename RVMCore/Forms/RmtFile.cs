using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore
{
    public class RmtFile
    {
        /// <summary>
        /// Full path of upload file.
        /// </summary>
        public string FullFilePath { get; set; }
        /// <summary>
        /// Show if folder name has change.
        /// </summary>
        public bool IsFatherUpdate { get; set; } = false;
        /// <summary>
        /// Present only <see cref="IsFatherUpdate"/> is true.
        /// </summary>
        public string OldFatherName { get; set; }
        /// <summary>
        /// Identfy if this object will be processed even the file has not been edited.
        /// </summary>
        public bool ProcessAnyway { get; set; } = false;

        public RmtFile(string path,bool isUpdate,string oldFather,bool processNow)
        {
            this.FullFilePath = path;
            this.IsFatherUpdate = isUpdate;
            this.OldFatherName = oldFather;
            this.ProcessAnyway = processNow;
        }
        public RmtFile(string path)
        {
            this.FullFilePath = path;
        }
        public RmtFile(string path, bool processNow)
        {
            this.FullFilePath = path;
            this.ProcessAnyway = processNow;
        }
        public RmtFile()
        {
        }
    }
}
