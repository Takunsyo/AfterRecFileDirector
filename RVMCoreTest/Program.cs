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
            Application.Run(new Form1());
            //var wpfwindow = new RVMCore.Forms.Uploader();
            //var wpfwindow = new RVMCore.Forms.CloudViewer();
            //var wpfwindow = new RVMCore.MirakurunWarpper.MirakurunViewer();
            //ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            //if (wpfwindow.ShowDialog() == true) return;
            //Application.Run(new RVMCore.Forms.Window1());
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
