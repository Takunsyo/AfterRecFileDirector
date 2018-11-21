using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a chat.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Unique identifier for this chat. 
        /// This number may be greater than 32 bits and some programming 
        /// languages may have difficulty/silent defects in interpreting it. 
        /// But it is smaller than 52 bits, so a signed 64 bit integer or 
        /// double-precision float type are safe for storing this identifier.
        /// </summary>
        public Int64 id { get; set; }
        /// <summary>
        /// Type of chat, can be either “private”, “group”, “supergroup” or “channel”
        /// </summary>
        public ChatType type { get; set; }
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
        /// Optional. Title, for supergroups, channels and group chats
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Optional. True if a group has ‘All Members Are Admins’ enabled.
        /// </summary>
        public bool all_members_are_administrators { get; set; }
        /// <summary>
        /// Optional. Chat photo. Returned only in getChat.
        /// </summary>
        public ChatPhoto photo { get; set; }
        /// <summary>
        /// Optional. Description, for supergroups and channel chats. Returned only in getChat.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Optional. Chat invite link, for supergroups and channel chats. Returned only in getChat.
        /// </summary>
        public string invite_link { get; set; }
        /// <summary>
        /// Optional. Pinned message, for supergroups and channel chats. Returned only in getChat.
        /// </summary>
        public Message pinned_message { get; set; }
        /// <summary>
        /// Optional. For supergroups, name of group sticker set. Returned only in getChat.
        /// </summary>
        public string sticker_set_name { get; set; }
        /// <summary>
        /// Optional. True, if the bot can change the group sticker set. Returned only in getChat.
        /// </summary>
        public bool can_set_sticker_set { get; set; }
    }

    public enum ChatType
    {
        @private, group, supergroup, channel
    }
}
