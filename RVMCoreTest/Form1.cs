using RVMCore.MirakurunWarpper;
using RVMCore.MirakurunWarpper.Apis;
using RVMCore.EPGStationWarpper.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace RVMCoreTest
{
    public partial class Form1 : Form
    {
        //EPGAccess mainAccess;
        public Form1()
        {
            InitializeComponent();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        //Thread mThread;
        CancellationTokenSource ct = new CancellationTokenSource();
        MirakurunService access = new MirakurunService("http://127.0.0.1:40772");

        private void Form1_Load(object sender, EventArgs e)
        {
            //var stuff = access.GetEvents();
            //foreach(var i in stuff)
            //{
            //    Debug.WriteLine(i.type);
            ////}
            //access.EventRecived += mh;
            //access.SubscribeEvents();
            //access.LogRecived += log;
            //access.SubscribeLogs();
            var bot = new RVMCore.TelgeramBot.Bot("685379411:AAFdW8jh1t8uhr5N7leYM8pW-ldtZLL807Y");
            bot.GetUpdates();
            var me=bot.GetMe();
            MessageBox.Show(me.id.ToString());
        }

        private void mh(object sender, Event events)
        {
            if (events is null) return;
            switch (events.resource)
            {
                case ResourceType.program:
                    Debug.WriteLine(((RVMCore.MirakurunWarpper.Apis.Program)events.Data).name);
                    break;
                case ResourceType.service:
                    Debug.WriteLine(((Service)events.Data).name);
                    break;
                case ResourceType.tuner:
                    Debug.WriteLine(((Tuner)events.Data).name);
                    break;
            }
        }

        private void log(object sender, string log)
        {
            Debug.Write(log);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ct.Cancel();
            access.StopSubscribeLogs();
        }
    }
}
