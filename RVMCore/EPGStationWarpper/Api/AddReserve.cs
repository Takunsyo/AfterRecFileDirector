namespace RVMCore.EPGStationWarpper.Api
{
    public class AddReserve
    {
        public long programId { get; set; }
        public Reserve.AddReserveOption option { get; set; }
        public Reserve.RuleEncode encode { get; set; }

    }
}
