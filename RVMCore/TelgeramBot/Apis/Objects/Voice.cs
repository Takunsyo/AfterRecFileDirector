using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a voice note.
    /// </summary>
    public class Voice : File
    {
        /// <summary>
        /// Duration of the audio in seconds as defined by sender
        /// </summary>
        public int duration { get; set; }
    }
}
