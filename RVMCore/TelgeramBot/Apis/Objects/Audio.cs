using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents an audio file to be treated as music by the Telegram clients.
    /// </summary>
    public class Audio : File
    {
        /// <summary>
        /// Duration of the audio in seconds as defined by sender
        /// </summary>
        public long duration { get; set; }
        /// <summary>
        /// Optional. Performer of the audio as defined by sender or by audio tags
        /// </summary>
        public string performer { get; set; }
        /// <summary>
        /// Optional. Title of the audio as defined by sender or by audio tags
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Optional. Thumbnail of the album cover to which the music file belongs
        /// </summary>
        public PhotoSize thumb { get; set; }
    }
}
