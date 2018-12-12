namespace RVMCore.EPGStationWarpper.Api
{
    public class Reserve
    {
        public Program program { get; set; }
        public long ruleId { get; set; }
        public AddReserveOption option { get; set; }
        public RuleEncode encode { get; set; }

        public class AddReserveOption
        {
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
