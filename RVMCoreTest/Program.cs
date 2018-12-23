using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RVMCore;
using System.Windows.Forms.Integration;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace RVMCoreTest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            if (Debugger.IsAttached)
                Console.SetOut(new DebugWriter());
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            //var wpfwindow = new RVMCore.Forms.Uploader();
            //var wpfwindow = new RVMCore.MasterView.MasterViewControl();
            //var wpfwindow = new RVMCore.Forms.CloudViewer();
            //var wpfwindow = new RVMCore.MasterView.Setting();
            //RVMCore.TVAFT.SortFile(new string[] { "-mirakurun" });
            //RVMCore.TVAFT.SortFile(new string[] { "-upload" });
            //RVMCore.TVAFT.SortFile(new string[] { "-cloud" });
            //RVMCore.TVAFT.SortFile(new string[] { "-main" });
            //Application.Run(new RVMCore.Forms.Window1());
            //var client = new RVMCore.PipeClient<RmtFile>();
            //client.Send(new RmtFile(@"D:\SHARED\testfile.mp4"), "RVMCoreUploader");
            //var access = new RVMCore.EPGStationWarpper.EPGAccess(SettingObj.Read());
            //var allItem = access.GetRules();
            //foreach(var i in allItem)
            //{
            //    Debug.WriteLine(i.keyword);
            //}
            //var mirakurun = new RVMCore.MirakurunWarpper.MirakurunService("http://192.168.0.2:40772/");
            //var wpfwindow = new RVMCore.MasterView.MirakurunLogView(mirakurun);
            //ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            //if (wpfwindow.ShowDialog() == true) return;
            foreach(var i in Share.GetFibonacciSequence())
            {
                if (i > 1000) break;
                Console.WriteLine(i.ToString());
            }

        }
    }

    class DebugWriter : TextWriter
    {
        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
            base.WriteLine(value);
        }

        public override void Write(string value)
        {
            Debug.Write(value);
            base.Write(value);
        }
        
        public override Encoding Encoding {
            get
            {
                return System.Text.Encoding.Unicode;
            }

        }
    }
}
