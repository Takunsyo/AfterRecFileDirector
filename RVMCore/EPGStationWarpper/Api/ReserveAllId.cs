using System.Collections.Generic;

namespace RVMCore.EPGStationWarpper.Api
{
    public class ReserveAllId
    {
        public IEnumerable<ReserveAllItem> reserves { get; set; }
        public IEnumerable<ReserveAllItem> conflicts { get; set; }
        public IEnumerable<ReserveAllItem> skips { get; set; }
        public IEnumerable<ReserveAllItem> overlaps { get; set; }
    }
}
