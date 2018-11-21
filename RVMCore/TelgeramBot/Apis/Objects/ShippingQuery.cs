using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object contains information about an incoming shipping query.
    /// </summary>
    public class ShippingQuery
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
        /// Bot specified invoice payload
        /// </summary>
        public string invoice_payload { get; set; }
        /// <summary>
        /// User specified shipping address
        /// </summary>
        public ShippingAddress shipping_address { get; set; }
    }
}
