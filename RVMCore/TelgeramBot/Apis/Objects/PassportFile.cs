using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a file uploaded to Telegram Passport. <para/>Currently all Telegram Passport files are in JPEG format when decrypted and don't exceed 10MB.
    /// </summary>
    public class PassportFile
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        public string file_id { get; set; }
        /// <summary>
        /// File size
        /// </summary>
        public int file_size { get; set; }
        /// <summary>
        /// Unix time when the file was uploaded
        /// </summary>
        public long file_date { get; set; }
    }
}
