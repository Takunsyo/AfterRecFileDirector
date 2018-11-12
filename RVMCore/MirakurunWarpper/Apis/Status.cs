using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{

    public class Status
    {
        public Process process { get; set; }
        public Epg epg { get; set; }
        public Streamcount streamCount { get; set; }
        public Errorcount errorCount { get; set; }
        public Timeraccuracy timerAccuracy { get; set; }
    }

    public class Process
    {
        public string arch { get; set; }
        public string platform { get; set; }
        public Versions versions { get; set; }
        public int pid { get; set; }
        public Memoryusage memoryUsage { get; set; }
    }

    public class Versions
    {
        public string http_parser { get; set; }
        public string node { get; set; }
        public string v8 { get; set; }
        public string uv { get; set; }
        public string zlib { get; set; }
        public string ares { get; set; }
        public string modules { get; set; }
        public string nghttp2 { get; set; }
        public string napi { get; set; }
        public string openssl { get; set; }
        public string icu { get; set; }
        public string unicode { get; set; }
        public string cldr { get; set; }
        public string tz { get; set; }
    }

    public class Memoryusage
    {
        public int rss { get; set; }
        public int heapTotal { get; set; }
        public int heapUsed { get; set; }
        public int external { get; set; }
    }

    public class Epg
    {
        public int[] gatheringNetworks { get; set; }
        public int storedEvents { get; set; }
    }

    public class Streamcount
    {
        public int tunerDevice { get; set; }
        public int tsFilter { get; set; }
        public int decoder { get; set; }
    }

    public class Errorcount
    {
        public int uncaughtException { get; set; }
        public int bufferOverflow { get; set; }
        public int tunerDeviceRespawn { get; set; }
    }

    public class Timeraccuracy
    {
        public int last { get; set; }
        public Accuracy m1 { get; set; }
        public Accuracy m5 { get; set; }
        public Accuracy m15 { get; set; }
        public class Accuracy
        {
            public double avg { get; set; }
            public double min { get; set; }
            public double max { get; set; }
        }
    }


}
