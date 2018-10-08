using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
