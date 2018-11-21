using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a general file (as opposed to <see cref="PhotoSize"/>, <see cref="Voice"/> and <see cref="Audio"/>)
    /// </summary>
    public class Document : File
    {
        /// <summary>
        /// Optional. Thumbnail of the album cover to which the music file belongs
        /// </summary>
        public PhotoSize thumb { get; set; }
        /// <summary>
        /// Optional. Original filename as defined by sender
        /// </summary>
        public string file_name { get; set; }
    }
}
