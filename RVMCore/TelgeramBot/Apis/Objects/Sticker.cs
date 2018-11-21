using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a sticker.
    /// </summary>
    public class Sticker
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        public string file_id { get; set; }
        /// <summary>
        /// Sticker width
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Sticker height
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// Optional. Sticker thumbnail in the .webp or .jpg format
        /// </summary>
        public PhotoSize thumb { get; set; }
        /// <summary>
        /// Optional. Emoji associated with the sticker
        /// </summary>
        public string emoji { get; set; }
        /// <summary>
        /// Optional. Name of the sticker set to which the sticker belongs
        /// </summary>
        public string set_name { get; set; }
        /// <summary>
        /// Optional. For mask stickers, the position where the mask should be placed
        /// </summary>
        public MaskPosition mask_position { get; set; }
        /// <summary>
        /// Optional. File size
        /// </summary>
        public long file_size { get; set; }
    }
}
