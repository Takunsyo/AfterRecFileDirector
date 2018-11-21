using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object contains basic information about an invoice.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Product name
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Product description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Unique bot deep-linking parameter that can be used to generate this invoice
        /// </summary>
        public string start_parameter { get; set; }
        /// <summary>
        /// Three-letter ISO 4217 currency code
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
        /// </summary>
        public int total_amount { get; set; }
    }
}
