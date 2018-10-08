using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    ///  EPGStation API
    /// 'server addr'/recorded/{id}
    /// </summary>
    [JsonObject]
    public class EPGchannel : EPGDefault
    {
        public long id { get; set; }
        public int serviceId { get; set; }
        public int networkId { get; set; }
        public string name { get; set; }
        public int remoteControlKeyId { get; set; }
        public bool hasLogoData { get; set; }
        public string channelType { get; set; }
        public string channel { get; set; }
        public int type { get; set; }
    }
}
