using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a chat photo.
    /// </summary>
    public class ChatPhoto
    {
        /// <summary>
        /// Unique file identifier of small (160x160) chat photo. This file_id can be used only for photo download.
        /// </summary>
        public string small_file_id { get; set; }
        /// <summary>
        /// Unique file identifier of big (640x640) chat photo. This file_id can be used only for photo download.
        /// </summary>
        public string big_file_id { get; set; }
    }
}
