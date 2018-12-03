using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TVRecLiveService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
#if (!DEBUG)
           ServiceBase[] ServicesToRun;
           ServicesToRun = new ServiceBase[] 
	   { 
	        new ServiceMain() 
	   };
           ServiceBase.Run(ServicesToRun);
#else
            ServiceMain myServ = new ServiceMain();
            myServ.Process();
            // here Process is my Service function
            // that will run when my service onstart is call
            // you need to call your own method or function name here instead of Process();
#endif
        }
    }
}
