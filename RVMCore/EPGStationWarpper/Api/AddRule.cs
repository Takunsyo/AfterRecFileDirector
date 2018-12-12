using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper.Api
{
    public class AddRule
    {

        public RuleSearch search { get; set; }
        public RuleOption option { get; set; }
        public RuleEncode encode { get; set; }

        public class RuleSearch
        {
            public string keyword { get; set; }
            public string ignoreKeyword { get; set; }
            public bool keyCS { get; set; }
            public bool keyRegExp { get; set; }
            public bool title { get; set; }
            public bool description { get; set; }
            public bool extended { get; set; }
            public bool GR { get; set; }
            public bool BS { get; set; }
            public bool CS { get; set; }
            public bool SKY { get; set; }
            public int station { get; set; }
            public int genrelv1 { get; set; }
            public int genrelv2 { get; set; }
            public int startTime { get; set; }
            public int timeRange { get; set; }
            public int week { get; set; }
            public bool isFree { get; set; }
            public int durationMin { get; set; }
            public int durationMax { get; set; }
            public bool avoidDuplicate { get; set; }
            public int periodToAvoidDuplicate { get; set; }
        }

        public class RuleOption
        {
            public bool enable { get; set; }
            public string directory { get; set; }
            public string recordedFormat { get; set; }
        }

        public class RuleEncode
        {
            public int mode1 { get; set; }
            public string directory1 { get; set; }
            public int mode2 { get; set; }
            public string directory2 { get; set; }
            public int mode3 { get; set; }
            public string directory3 { get; set; }
            public bool delTs { get; set; }
        }

    }
}
