﻿using System;
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
using System.Data;
using System.Threading;

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

            //var mdata = new Database("127.0.0.1", "laoxiaoms", "76151319");
            //var wpfwindow = new RVMCore.MasterView.Setting();
            //var result = wpfwindow.ShowDialog();
            //if (result ?? false)
            //{
            //    Console.WriteLine("OK");
            //}
            //RVMCore.TVAFT.SortFile(new string[] { "-mirakurun" });
            RVMCore.TVAFT.SortFile(new string[] { "-rcdbview" });
            //RVMCore.TVAFT.SortFile(new string[] { "-epgstation", "-id", "278" });
            //RVMCore.TVAFT.SortFile(new string[] { "-upload" });
            //RVMCore.TVAFT.SortFile(new string[] { "-cloud" });
            //RVMCore.TVAFT.SortFile(new string[] { "-main" });
            //Application.Run(new RVMCore.Forms.Window1());
            //RVMCore.TVAFT.TestMethod();
            //var id = Database.GenerateID();
            //var t = mdata.ValidDatabase();
            //Console.WriteLine();
            //mdata.CreateDatabase();
            //var serv =new RVMCore.MirakurunWarpper.MirakurunService(SettingObj.Read());
            //System.IO.File.Delete(@"D:\hoge.m2ts");
            //var chs = serv.GetChannels(RVMCore.MirakurunWarpper.ChannelType.BS);
            //var ch = chs.ToList()[1];
            //var token = new CancellationTokenSource();
            //serv.StreamServiceToFile(ch.services.First().id, @"D:\hoge.m2ts", token.Token).Wait();
            //var resetter = new ManualResetEvent(false);
            //resetter.Reset();
            //ThreadPool.QueueUserWorkItem(async x=> { try {await serv.StreamServiceToFile(ch.services.First().id, @"D:\hoge.m2ts", token.Token); } finally { resetter.Set(); } });
            //resetter.WaitOne();
            //var dataTable = new DataTable();
            //mdata.LoadData(ref dataTable, false);
            //mdata.LoadData(ref dataTable, true);
            //foreach (var i in dataTable.Rows)
            //{
            //    Console.WriteLine(i.ToString());
            //}

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
