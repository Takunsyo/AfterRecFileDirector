using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a venue.
    /// </summary>
    public class Venue
    {
        /// <summary>
        /// Venue location
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// Name of the venue
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Address of the venue
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// Optional. Foursquare identifier of the venue
        /// </summary>
        public string foursquare_id { get; set; }
        /// <summary>
        /// Optional. Foursquare type of the venue. 
        /// <para/>(For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)
        /// </summary>
        public string foursquare_type { get; set; }
    }
}
