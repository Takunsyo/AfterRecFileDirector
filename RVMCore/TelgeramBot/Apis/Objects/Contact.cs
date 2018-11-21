using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a phone contact.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Contact's phone number
        /// </summary>
        public string phone_number { get; set; }
        /// <summary>
        /// Contact's first name
        /// </summary>
        public string first_name { get; set; }
        /// <summary>
        /// Optional. Contact's last name
        /// </summary>
        public string last_name { get; set; }
        /// <summary>
        /// Optional. Contact's user identifier in Telegram
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// Optional. Additional data about the contact in the form of a vCard
        /// </summary>
        public string vcard { get; set; }
    }
}
