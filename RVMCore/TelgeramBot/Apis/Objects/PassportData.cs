using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// Contains information about Telegram Passport data shared with the bot by the user.
    /// </summary>
    public class PassportData
    {
        /// <summary>
        /// Array with information about documents and other Telegram Passport elements that was shared with the bot
        /// </summary>
        public List<EncryptedPassportElement> data { get; set; }
        /// <summary>
        /// Encrypted credentials required to decrypt the data
        /// </summary>
        public EncryptedCredentials credentials { get; set; }
    }
}
