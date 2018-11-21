using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a video file.
    /// </summary>
    public class Video : File
    {
        /// <summary>
        /// Video width as defined by sender
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Video height as defined by sender
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// Duration of the video in seconds as defined by sender
        /// </summary>
        public long duration { get; set; }
        /// <summary>
        /// Optional. Video thumbnail
        /// </summary>
        public PhotoSize thumb { get; set; }
    }
}
