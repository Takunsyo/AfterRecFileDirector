using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// Contains data required for decrypting and authenticating <see cref="EncryptedPassportElement"/>. <para/>See the Telegram Passport Documentation for a complete description of the data decryption and authentication processes
    /// </summary>
    public class EncryptedCredentials
    {
        /// <summary>
        /// Base64-encoded encrypted JSON-serialized data with unique user's payload, data hashes and secrets required for <see cref="EncryptedPassportElement"/> decryption and authentication
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// Base64-encoded data hash for data authentication
        /// </summary>
        public string hash { get; set; }
        /// <summary>
        /// Base64-encoded secret, encrypted with the bot's public RSA key, required for data decryption
        /// </summary>
        public string secret { get; set; }
    }
}
