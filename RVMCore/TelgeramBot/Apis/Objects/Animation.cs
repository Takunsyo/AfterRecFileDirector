using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents an animation file (GIF or H.264/MPEG-4 AVC video without sound).
    /// </summary>
    public class Animation : Video
    {
        /// <summary>
        /// Optional. Original animation filename as defined by sender
        /// </summary>
        public string file_name { get; set; }
    }
}
