using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.EPGStationWarpper.Api
{
    /// <summary>
    /// EPGStation api's basic response normally represent of error during process.
    /// </summary>
    [JsonObject]
    public class EPGDefault
    {
        public int code { get; set; }

        public string message { get; set; }

        public string errors { get; set; }
                    
    }
}
