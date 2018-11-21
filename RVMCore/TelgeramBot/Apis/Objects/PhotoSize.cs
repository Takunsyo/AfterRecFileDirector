using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents one size of a photo or a file / sticker thumbnail.
    /// </summary>
    public class PhotoSize
    {
        /// <summary>
        /// 	Unique identifier for this file
        /// </summary>
        public string file_id { get; set; }
        /// <summary>
        /// Photo width
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Photo height
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// Optional. File size
        /// </summary>
        public long file_size { get; set; }
    }
}
