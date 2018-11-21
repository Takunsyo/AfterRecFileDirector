using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// /This object represents a shipping address.
    /// </summary>
    public class ShippingAddress
    {
        /// <summary>
        /// ISO 3166-1 alpha-2 country code
        /// </summary>
        public string country_code { get; set; }
        /// <summary>
        /// State, if applicable
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// City
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// First line for the address
        /// </summary>
        public string street_line1 { get; set; }
        /// <summary>
        /// Second line for the address
        /// </summary>
        public string street_line2 { get; set; }
        /// <summary>
        /// Address post code
        /// </summary>
        public string post_code { get; set; }
    }
}
