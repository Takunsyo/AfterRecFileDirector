using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents an incoming update.
    /// <para/>At most one of the optional parameters can be present in any given update.
    /// </summary>
    public class Update
    {
        /// <summary>
        /// The update‘s unique identifier. Update identifiers start from a certain positive number and increase sequentially. This ID becomes especially handy if you’re using Webhooks, since it allows you to ignore repeated updates or to restore the correct update sequence, should they get out of order. If there are no new updates for at least a week, then identifier of the next update will be chosen randomly instead of sequentially.
        /// </summary>
        public int update_id { get; set; }
        /// <summary>
        /// Optional. New incoming message of any kind — text, photo, sticker, etc.
        /// </summary>
        public Message message { get; set; }
        /// <summary>
        /// Optional. New version of a message that is known to the bot and was edited
        /// </summary>
        public Message edited_message { get; set; }
        /// <summary>
        /// Optional. New incoming channel post of any kind — text, photo, sticker, etc.
        /// </summary>
        public Message channel_post { get; set; }
        /// <summary>
        /// Optional. New version of a channel post that is known to the bot and was edited
        /// </summary>
        public Message edited_channel_post { get; set; }
        /// <summary>
        /// Optional. New incoming inline query
        /// </summary>
        public InlineQuery inline_query { get; set; }
        /// <summary>
        /// Optional. The result of an inline query that was chosen by a user and sent to their chat partner. Please see our documentation on the feedback collecting for details on how to enable these updates for your bot.
        /// </summary>
        public ChosenInlineResult chosen_inline_result { get; set; }
        /// <summary>
        /// Optional. New incoming callback query
        /// </summary>
        public CallbackQuery callback_query { get; set; }
        /// <summary>
        /// Optional. New incoming shipping query. Only for invoices with flexible price
        /// </summary>
        public ShippingQuery shipping_query { get; set; }
        /// <summary>
        /// Optional. New incoming pre-checkout query. Contains full information about checkout
        /// </summary>
        public PreCheckoutQuery pre_checkout_query { get; set; }
    }
}
