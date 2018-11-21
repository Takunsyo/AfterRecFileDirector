using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a game. Use BotFather to create and edit games, their short names will act as unique identifiers.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Title of the game
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Description of the game
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Photo that will be displayed in the game message in chats.
        /// </summary>
        public List<PhotoSize> photo { get; set; }
        /// <summary>
        /// Optional. Brief description of the game or high scores included in the game message. Can be automatically edited to include current high scores for the game when the bot calls setGameScore, or manually edited using editMessageText. 0-4096 characters.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Optional. Special entities that appear in text, such as usernames, URLs, bot commands, etc.
        /// </summary>
        public List<MessageEntity> text_entities { get; set; }
        /// <summary>
        /// Optional. Animation that will be displayed in the game message in chats. Upload via BotFather
        /// </summary>
        public Animation animation { get; set; }
    }
}
