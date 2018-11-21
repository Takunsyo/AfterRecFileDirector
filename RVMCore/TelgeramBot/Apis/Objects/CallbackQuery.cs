using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents an incoming callback query from a callback button in an inline keyboard. If the button that originated the query was attached to a message sent by the bot, the field message will be present. If the button was attached to a message sent via the bot (in inline mode), the field inline_message_id will be present. Exactly one of the fields data or game_short_name will be present.
    /// </summary>
    public class CallbackQuery
    {
        /// <summary>
        /// Unique identifier for this query
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Sender
        /// </summary>
        public User from { get; set; }
        /// <summary>
        /// Optional. Message with the callback button that originated the query. Note that message content and message date will not be available if the message is too old
        /// </summary>
        public Message message { get; set; }
        /// <summary>
        /// Optional. Identifier of the message sent via the bot in inline mode, that originated the query.
        /// </summary>
        public string inline_message_id { get; set; }
        /// <summary>
        /// Global identifier, uniquely corresponding to the chat to which the message with the callback button was sent. Useful for high scores in games.
        /// </summary>
        public string chat_instance { get; set; }
        /// <summary>
        /// Optional. Data associated with the callback button. Be aware that a bad client can send arbitrary data in this field.
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// Optional. Short name of a Game to be returned, serves as the unique identifier for the game
        /// </summary>
        public string game_short_name { get; set; }
    }
}
