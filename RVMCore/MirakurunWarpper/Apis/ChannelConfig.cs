using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{
    public class ChannelConfig
    {
        public string name { get; set; }
        public string type { get; set; }
        public string channel { get; set; }
        public int serviceId { get; set; }
        public string satelite { get; set; }
        public int space { get; set; }
        public bool isDisabled { get; set; }
    }

}
