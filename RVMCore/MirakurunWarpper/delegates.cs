using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RVMCore.MirakurunWarpper.Apis;

namespace RVMCore.MirakurunWarpper
{
    public delegate void EventRecivedHandler(object sender, Event events);
    public delegate void LogRecivedHandler(object sender, string log);
}
