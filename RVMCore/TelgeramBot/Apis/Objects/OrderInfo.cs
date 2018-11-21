using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents information about an order.
    /// </summary>
    public class OrderInfo
    {
        /// <summary>
        /// Optional. User name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Optional. User's phone number
        /// </summary>
        public string phone_number { get; set; }
        /// <summary>
        /// Optional. User email
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Optional. User shipping address
        /// </summary>
        public ShippingAddress shipping_address { get; set; }
    }
}
