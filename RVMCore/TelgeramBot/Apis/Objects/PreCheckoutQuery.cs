using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object contains information about an incoming pre-checkout query.
    /// </summary>
    public class PreCheckoutQuery
    {
        /// <summary>
        /// Unique query identifier
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// User who sent the query
        /// </summary>
        public User from { get; set; }
        /// <summary>
        /// Three-letter ISO 4217 currency code
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
        /// </summary>
        public int total_amount { get; set; }
        /// <summary>
        /// Bot specified invoice payload
        /// </summary>
        public string invoice_payload { get; set; }
        /// <summary>
        /// Optional. Identifier of the shipping option chosen by the user
        /// </summary>
        public string shipping_option_id { get; set; }
        /// <summary>
        /// Optional. Order info provided by the user
        /// </summary>
        public OrderInfo order_info { get; set; }

    }
}
