using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{
    public class ServerConfig
    {
        public string path { get; set; }
        public int port { get; set; }
        public bool disableIPv6 { get; set; }
        public int logLevel { get; set; }
        public int maxLogHistory { get; set; }
        public int highWaterMark { get; set; }
        public int overflowTimeLimit { get; set; }
        public int maxBufferBytesBeforeReady { get; set; }
        public int eventEndTimeout { get; set; }
        public int programGCInterval { get; set; }
        public int epgGatheringInterval { get; set; }
        public int epgRetrievalTime { get; set; }
    }

}
