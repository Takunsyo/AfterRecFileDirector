﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// This object represents an incoming inline query. When the user sends an empty query, your bot could return some default or trending results.
    /// </summary>
    public class InlineQuery
    {
        /// <summary>
        /// Unique identifier for this query
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Sender
        /// </summary>
        public User from { get; set; }
        /// <summary>
        /// Optional. Sender location, only for bots that request user location
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// Text of the query (up to 512 characters)
        /// </summary>
        public string query { get; set; }
        /// <summary>
        /// Offset of the results to be returned, can be controlled by the bot
        /// </summary>
        public string offset { get; set; }
    }
}
