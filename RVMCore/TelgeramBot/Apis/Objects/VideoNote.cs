using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a video message (available in Telegram apps as of v.4.0).
    /// </summary>
    public class VideoNote : Voice
    {
        /// <summary>
        /// Video width and height (diameter of the video message) as defined by sender
        /// </summary>
        public long length { get; set; }
        /// <summary>
        /// Optional. Video thumbnail
        /// </summary>
        public PhotoSize thumb { get; set; }
    }
}
