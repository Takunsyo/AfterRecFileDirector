using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a file ready to be downloaded. 
    /// The file can be downloaded via api /file/bot<token>/<file_path>. 
    /// It is guaranteed that the link will be valid for at least 1 hour.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        public string file_id { get; set; }
        /// <summary>
        /// Optional. MIME type of the file as defined by sender
        /// </summary>
        public string mime_type { get; set; }
        /// <summary>
        /// Optional. File size
        /// </summary>
        public long file_size { get; set; }
        /// <summary>
        /// Optional. File path. Use https://api.telegram.org/file/bot<token>/<file_path> to get the file.
        /// </summary>
        public string file_path { get; set; }
    }
}
