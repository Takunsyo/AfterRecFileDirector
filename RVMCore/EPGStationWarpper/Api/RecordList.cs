using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper.Api
{
    class RecordList:EPGDefault
    {
        public IEnumerable<RecordedProgram> recorded { get; set; }
        public int total { get; set; }
    }
}
