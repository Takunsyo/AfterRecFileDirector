using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// Represents a result of an inline query that was chosen by the user and sent to their chat partner.
    /// </summary>
    public class ChosenInlineResult
    {
        /// <summary>
        /// The unique identifier for the result that was 
        /// </summary>
        public string result_id { get; set; }
        /// <summary>
        /// The user that chose the result
        /// </summary>
        public User from { get; set; }
        /// <summary>
        /// Optional. Sender location, only for bots that require user location
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// Optional. Identifier of the sent inline message. Available only if there is an inline keyboard attached to the message. Will be also received in callback queries and can be used to edit the message.
        /// </summary>
        public string inline_message_id { get; set; }
        /// <summary>
        /// The query that was used to obtain the result
        /// </summary>
        public string query { get; set; }
    }
}
