using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{

    public class TunerConfig
    {
        public string name { get; set; }
        public string[] types { get; set; }
        public string command { get; set; }
        public string dvbDevicePath { get; set; }
        public string remoteMirakurunHost { get; set; }
        public int remoteMirakurunPort { get; set; }
        public bool remoteMirakurunDecoder { get; set; }
        public string decoder { get; set; }
        public bool isDisabled { get; set; }
    }

}
