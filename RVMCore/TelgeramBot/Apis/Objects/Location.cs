using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents a point on the map.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Longitude as defined by sender
        /// </summary>
        public float longitude { get; set; }
        /// <summary>
        /// Latitude as defined by sender
        /// </summary>
        public float latitude { get; set; }
    }
}
