using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Unique message identifier inside this chat
        /// </summary>
        public int message_id { get; set; }
        /// <summary>
        /// Optional. Sender, empty for messages sent to channels
        /// </summary>
        public User from { get; set; }
        /// <summary>
        /// Conversation the message belongs to
        /// </summary>
        public Chat chat { get; set; }
        /// <summary>
        /// Date the message was sent in Unix time
        /// </summary>
        public long date { get; set; }
        /// <summary>
        /// Optional. For forwarded messages, sender of the original message
        /// </summary>
        public User forward_from { get; set; }
        /// <summary>
        /// Optional. For messages forwarded from channels, information about the original channel
        /// </summary>
        public Chat forward_from_chat { get; set; }
        /// <summary>
        /// Optional. For messages forwarded from channels, identifier of the original message in the channel
        /// </summary>
        public int forward_from_message_id { get; set; }
        /// <summary>
        /// Optional. For messages forwarded from channels, signature of the post author if present
        /// </summary>
        public string forward_signature { get; set; }
        /// <summary>
        /// Optional. For forwarded messages, date the original message was sent in Unix time
        /// </summary>
        public long forward_date { get; set; }
        /// <summary>
        /// Optional. For replies, the original message. Note that the Message object in this field will not contain further reply_to_message fields even if it itself is a reply.
        /// </summary>
        public Message reply_to_message { get; set; }
        /// <summary>
        /// Optional. Date the message was last edited in Unix time
        /// </summary>
        public long edit_date { get; set; }
        /// <summary>
        /// Optional. The unique identifier of a media message group this message belongs to
        /// </summary>
        public string media_group_id { get; set; }
        /// <summary>
        /// Optional. Signature of the post author for messages in channels
        /// </summary>
        public string author_signature { get; set; }
        /// <summary>
        /// Optional. For text messages, the actual UTF-8 text of the message, 0-4096 characters.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Optional. For text messages, special entities like usernames, URLs, bot commands, etc. that appear in the text
        /// </summary>
        public List<MessageEntity> entities { get; set; }
        /// <summary>
        /// Optional. For messages with a caption, special entities like usernames, URLs, bot commands, etc. that appear in the caption
        /// </summary>
        public List<MessageEntity> caption_entities { get; set; }
        /// <summary>
        /// Optional. Message is an audio file, information about the file
        /// </summary>
        public Audio audio { get; set; }
        /// <summary>
        /// Optional. Message is a general file, information about the file
        /// </summary>
        public Document document { get; set; }
        /// <summary>
        /// Optional. Message is an animation, information about the animation. For backward compatibility, when this field is set, the document field will also be set        /// </summary>
        public Animation animation { get; set; }
        /// <summary>
        /// Optional. Message is a game, information about the game. 
        /// <para>See description of <see cref="game"/></para>
        /// </summary>
        public Game game { get; set; }
        /// <summary>
        /// Optional. Message is a photo, available sizes of the photo
        /// </summary>
        public List<PhotoSize> photo { get; set; }
        /// <summary>
        /// Optional. Message is a sticker, information about the sticker
        /// </summary>
        public Sticker sticker { get; set; }
        /// <summary>
        /// Optional. Message is a video, information about the video
        /// </summary>
        public Video video { get; set; }
        /// <summary>
        /// Optional. Message is a voice message, information about the file
        /// </summary>
        public Voice voice { get; set; }
        /// <summary>
        /// Optional. Message is a video note, information about the video message
        /// </summary>
        public VideoNote video_note { get; set; }
        /// <summary>
        /// Optional. Caption for the audio, document, photo, video or voice, 0-1024 characters
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// Optional. Message is a shared contact, information about the contact
        /// </summary>
        public Contact contact { get; set; }
        /// <summary>
        /// Optional. Message is a shared location, information about the location
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// Optional. Message is a venue, information about the venue
        /// </summary>
        public Venue venue { get; set; }
        /// <summary>
        /// Optional. New members that were added to the group or supergroup and information about them (the bot itself may be one of these members)
        /// </summary>
        public List<User> new_chat_members { get; set; }
        /// <summary>
        /// Optional. A member was removed from the group, information about them (this member may be the bot itself)
        /// </summary>
        public User left_chat_member { get; set; }
        /// <summary>
        /// Optional. A chat title was changed to this value
        /// </summary>
        public string new_chat_title { get; set; }
        /// <summary>
        /// Optional. A chat photo was change to this value
        /// </summary>
        public List<PhotoSize> new_chat_photo { get; set; }
        /// <summary>
        /// Optional. Service message: the chat photo was deleted
        /// </summary>
        public bool delete_chat_photo { get; set; }
        /// <summary>
        /// Optional. Service message: the group has been created
        /// </summary>
        public bool group_chat_created { get; set; }
        /// <summary>
        /// Optional. Service message: the supergroup has been created. 
        /// This field can‘t be received in a message coming through updates, 
        /// because bot can’t be a member of a supergroup when it is created. 
        /// It can only be found in reply_to_message if someone replies to a 
        /// very first message in a directly created supergroup.
        /// </summary>
        public bool supergroup_chat_created { get; set; }
        /// <summary>
        /// Optional. Service message: the channel has been created. 
        /// This field can‘t be received in a message coming through updates, 
        /// because bot can’t be a member of a channel when it is created. 
        /// It can only be found in reply_to_message if someone replies to a 
        /// very first message in a channel.
        /// </summary>
        public bool channel_chat_created { get; set; }
        /// <summary>
        /// Optional. The group has been migrated to a supergroup with the 
        /// specified identifier. This number may be greater than 32 bits and 
        /// some programming languages may have difficulty/silent defects in 
        /// interpreting it. But it is smaller than 52 bits, so a signed 64 bit 
        /// integer or double-precision float type are safe for storing this identifier.
        /// </summary>
        public Int64 migrate_to_chat_id { get; set; }
        /// <summary>
        /// Optional. The supergroup has been migrated from a group with the 
        /// specified identifier. This number may be greater than 32 bits and 
        /// some programming languages may have difficulty/silent defects in 
        /// interpreting it. But it is smaller than 52 bits, so a signed 64 bit 
        /// integer or double-precision float type are safe for storing this identifier.
        /// </summary>
        public Int64 migrate_from_chat_id { get; set; }
        /// <summary>
        /// Optional. Specified message was pinned. 
        /// Note that the Message object in this field will not contain 
        /// further reply_to_message fields even if it is itself a reply.
        /// </summary>
        public Message pinned_message { get; set; }
        /// <summary>
        /// Optional. Message is an invoice for a <see cref="SuccessfulPayment"/>, information about the invoice.
        /// </summary>
        public Invoice invoice { get; set; }
        /// <summary>
        /// Optional. Message is a service message about a successful payment, information about the payment. 
        /// </summary>
        public SuccessfulPayment successful_payment { get; set; }
        /// <summary>
        /// Optional. The domain name of the website on which the user has logged in. 
        /// </summary>
        public string connected_website { get; set; }
        /// <summary>
        /// Optional. Telegram Passport data
        /// </summary>
        public PassportData passport_data { get; set; }
    }
}
