using System.Collections.Generic;

namespace RVMCore.EPGStationWarpper.Api
{
    class RecordList:EPGDefault
    {
        public IEnumerable<Program> recorded { get; set; }
        public int total { get; set; }
    }
}
