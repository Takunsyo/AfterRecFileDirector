using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a Telegram user or bot.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for this user or bot
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// True, if this user is a bot
        /// </summary>
        public bool is_bot { get; set; }
        /// <summary>
        /// User‘s or bot’s first name
        /// </summary>
        public string first_name { get; set; }
        /// <summary>
        /// Optional. User‘s or bot’s last name
        /// </summary>
        public string last_name { get; set; }
        /// <summary>
        /// Optional. User‘s or bot’s username
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// Optional. IETF language tag of the user's language
        /// </summary>
        public string language_code { get; set; }
    }
}
