using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.MirakurunWarpper.Apis
{
    class @default
    {
        public int code { get; set; }
        public string reason { get; set; }
        public Error[] errors { get; set; }

        public class Error
        {
            public string errorCode { get; set; }
            public string message { get; set; }
            public string location { get; set; }
        }

    }
}
