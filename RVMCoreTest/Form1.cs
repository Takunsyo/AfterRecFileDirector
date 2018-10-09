using RVMCore.EPGStationWarpper;
using RVMCore.EPGStationWarpper.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RVMCoreTest
{
    public partial class Form1 : Form
    {
        EPGAccess mainAccess;
        public Form1()
        {
            InitializeComponent();

            //load epgstation 
            mainAccess = new EPGAccess("laoxiaoms", "76151319", "192.168.0.2:40888", "");
            var c_list = mainAccess.GetChannels();
            listBox1.DisplayMember = "name";
            listBox1.DataSource = c_list;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string[] margs = textBox1.Text.Split(' ');
            //RVMCore.TVAFT.SortFile(margs);
            //MessageBox.Show(RVMCore.Share.FindTitle(textBox1.Text));
            //MessageBox.Show(RVMCore.Share.GetTimeSpan(DateTime.Now, DateTime.Now.AddMonths(-1)));

            //var tmp = mainAccess.GetRecordProgramByID(24).Serialize();
            //var tmp2 = RecordedProgram.Deserialize(tmp);
            //MessageBox.Show(tmp.Length.ToString() + tmp2.name);

            var main = new EPGMetaFile(this.mainAccess, 62);
            var tmp = main.GetBytes();
            if (!main.WtiteFile(@"E:\tmp.meta")) return;
            var tmp2 = EPGMetaFile.ReadFile(@"E:\tmp.meta");
            MessageBox.Show(tmp2.Meta.name);
            pictureBox1.Image = tmp2.ThumbImage;
            pictureBox2.Image = tmp2.Logo;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int length = 0;
            //if (((RecordedProgram)listBox1.SelectedItem).hasThumbnail)
            //{
            //    pictureBox1.Image = mainAccess.GetRecordedThumbnailByID(((RecordedProgram)listBox1.SelectedItem).id,out length);
            //    this.textBox1.Text = string.Format("[{0}]", length.ToString());
            //}
            //else pictureBox1.Image = null;

            pictureBox2.Image = mainAccess.GetChannelLogoByID(((EPGchannel)listBox1.SelectedItem).id,out length);
            this.textBox1.Text = string.Format("[{0}]", length.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show(((RVMCore.ProgramGenre)(1 << 11)).ToRecString());
        }

    }
}
