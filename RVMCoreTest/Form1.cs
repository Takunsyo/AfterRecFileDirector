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
        //EPGAccess mainAccess;
        public Form1()
        {
            InitializeComponent();

            //load epgstation 
            //mainAccess = new EPGAccess("laoxiaoms", "76151319", "192.168.0.2:40888", "");
            //var c_list = mainAccess.GetChannels();
            //listBox1.DisplayMember = "name";
            //listBox1.DataSource = c_list;
            //pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }
        RVMCore.GoogleWarpper.GoogleDrive mService = new RVMCore.GoogleWarpper.GoogleDrive();

        System.Threading.Thread mThread;
        private void button1_Click(object sender, EventArgs e)
        {
            //string[] margs = textBox1.Text.Split(' ');
            //RVMCore.TVAFT.SortFile(margs);
            //MessageBox.Show(RVMCore.Share.FindTitle(textBox1.Text));
            //MessageBox.Show(RVMCore.Share.GetTimeSpan(DateTime.Now, DateTime.Now.AddMonths(-1)));

            //var tmp = mainAccess.GetRecordProgramByID(24).Serialize();
            //var tmp2 = RecordedProgram.Deserialize(tmp);
            //MessageBox.Show(tmp.Length.ToString() + tmp2.name);

            //var main = new EPGMetaFile(this.mainAccess, 62);
            //var tmp = main.GetBytes();
            //if (!main.WtiteFile(@"E:\tmp.meta")) return;
            //var tmp2 = EPGMetaFile.ReadFile(@"E:\tmp.meta");
            //MessageBox.Show(tmp2.Meta.name);
            //pictureBox1.Image = tmp2.ThumbImage;
            //pictureBox2.Image = tmp2.Logo;
            //string path = @"E:\tmp\tmp.meta";
            //MessageBox.Show(System.IO.Path.GetExtension(path).ToLower() == (".meta") ? path : System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path) + ".meta"));
            //mainAccess.DeleteRecordByID(65);

            //Google get folder list

            //var i = mService.GetGoogleFolderByID(mService.Root.Id);
            ////var root = m_root.Execute();
            ////MessageBox.Show(root.Name + "\n" + root.Id);
            ////var fList = mService.Service.Files.List();
            ////fList.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false and 'me' in owners";
            //////fList.Q = "'root' in ";
            ////fList.Spaces = "drive";
            ////fList.Fields = "files(id, name, parents)";
            ////fList.SupportsTeamDrives = false;
            //////var rep =fList.Execute();
            ////foreach(Google.Apis.Drive.v3.Data.File i in rep.Files)
            ////{
            //    MessageBox.Show(i.Name + "\n ID=" + i.Id + "\n Type=" + i.MimeType + "\n Parents: \n" + getStr(i.Parents));
            //}

            //string local = @"E:\1[アニメ類]\[Q4'18,Q4'18]宇宙戦艦ティラミスⅡ\[20181009]ＴＯＫＹＯ　ＭＸ１[宇宙戦艦ティラミスⅡ #2「REUNION／NEO UNIVERSE…」][lxsrc].ts";
            List<string> fList = new List<string>();
            using (var mdlg = new OpenFileDialog())
            {
                mdlg.CheckFileExists = true;
                mdlg.Filter= "TransPortStream|*.ts";
                mdlg.InitialDirectory = @"E:\1[アニメ類]\";
                mdlg.Multiselect = true;
                if (mdlg.ShowDialog() == DialogResult.OK)
                {
                    fList.AddRange( mdlg.FileNames);
                }
                else return;
            }

            mService.MaxBytesPerSecond = 650 * 1024;
            //var exi = mService.RemoteFileExists(local, remot);
            //MessageBox.Show(exi.ToString());
            System.Threading.ThreadStart work = new System.Threading.ThreadStart(() =>
            {
                foreach(string local in fList) { 
                    string remot = System.IO.Path.GetDirectoryName(local).Replace(System.IO.Path.GetPathRoot(local), @"\EPGRecords\");
                    this.Invoke(new Action(() => {
                        this.Text = "[Uploading]" + System.IO.Path.GetFileName(local);
                        this.textBox1.Text = local;
                        this.textBox1.ReadOnly = true;
                        button1.Enabled = false;
                        lStatus.Text = "Initialize upload (MD5 File Checking...)";
                        this.progressBar1.Value = 0;
                        this.progressBar1.Style = ProgressBarStyle.Marquee;
                    }));
                    if (!System.IO.File.Exists(mService.GetUploadStatusPath(local)))
                    {
                        if (!mService.RemoteFileExists(local, remot))mService.UploadResumable(local, remot);
                    }
                    else mService.UploadResumable(local, remot);
                    button1.Invoke(new Action(() => { button1.Enabled = true; }));
                    lStatus.Invoke(new Action(() => { lStatus.Text = "Done."; }));
                }
            });
            mThread = new System.Threading.Thread(work);
            mThread.Start();
        }

        private string getStr(IList<string> input)
        {
            if (input == null) return "";
            string tmp = "";
            foreach(string i in input)
            {
                if (tmp != "") tmp += "\n";
                tmp += i;
            }
            return tmp;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int length = 0;
            ////if (((RecordedProgram)listBox1.SelectedItem).hasThumbnail)
            ////{
            ////    pictureBox1.Image = mainAccess.GetRecordedThumbnailByID(((RecordedProgram)listBox1.SelectedItem).id,out length);
            ////    this.textBox1.Text = string.Format("[{0}]", length.ToString());
            ////}
            ////else pictureBox1.Image = null;

            //pictureBox2.Image = mainAccess.GetChannelLogoByID(((EPGchannel)listBox1.SelectedItem).id,out length);
            //this.textBox1.Text = string.Format("[{0}]", length.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(((RVMCore.ProgramGenre)(1 << 11)).ToRecString());
            this.progressBar1.Font = this.Font;
            mService.UpdateProgressChanged += new RVMCore.GoogleWarpper.UpdateProgress((long value, long maxmum, int y) => {
                if (this.progressBar1.InvokeRequired)
                {
                    Action act = new Action(() => {
                        if(value >=0 && maxmum > 0) {
                            this.progressBar1.Style = ProgressBarStyle.Continuous;
                            this.progressBar1.Maximum = (maxmum > int.MaxValue) ? (int)(maxmum/2): (int)maxmum;
                            this.progressBar1.Value = (maxmum > int.MaxValue) ? (int)(value/2):(int)value;
                            string mstr = "[" + getSizeString(value) + "/" + getSizeString(maxmum) + "]" + getSizeString(y) + "/s";
                            var mSize = this.progressBar1.CreateGraphics().MeasureString(mstr, this.Font);
                            var mPos = new PointF((this.progressBar1.Height - mSize.Height) / 2, (this.progressBar1.Width - mSize.Width) / 2);
                            this.progressBar1.CreateGraphics().DrawString(mstr, this.Font, new SolidBrush(this.ForeColor), mPos);
                        }
                    });
                    this.progressBar1.Invoke(act);
                }
                if (this.lStatus.InvokeRequired)
                {
                    Action act = new Action(() => {
                        this.lStatus.Text = "["+ getSizeString(value) +"/" +getSizeString(maxmum)+"]" + getSizeString(y)+"/s";
                    });
                    this.lStatus.Invoke(act);
                }
            });
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if(mThread!=null && mThread.IsAlive)
            {
                if(mThread.ThreadState == System.Threading.ThreadState.Suspended || mThread.ThreadState == System.Threading.ThreadState.SuspendRequested)
                {
                    mThread.Resume();
                    this.progressBar1.State = VistaProgressBar.vState.Normal;
                }
                else
                {
                    mThread.Suspend();
                    this.progressBar1.State = VistaProgressBar.vState.Pause;
                }
            }
        }

        private string getSizeString(long size)
        {
            string tmp = "";
            if (size > 1024 * 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024 / 1024).ToString("F2") + " Gb";
            }
            else if (size > 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024).ToString("F2") + " Mb";
            }
            else if (size > 1024)
            {
                tmp = ((float)size / 1024).ToString("F2") + " Kb";
            }
            else
            {
                tmp = (size).ToString() + " Byte";
            }
            return tmp;
        }

    }

    [ToolboxBitmap(typeof(System.Windows.Forms.ProgressBar))]
    public class VistaProgressBar : System.Windows.Forms.ProgressBar
    {
        public delegate void StateChangedHandler(object source, vState State);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        private vState _State = vState.Normal;

        public enum vState { Normal, Pause, Error }

        private const int WM_USER = 0x400;
        private const int PBM_SETSTATE = WM_USER + 16;

        private const int PBST_NORMAL = 0x0001;
        private const int PBST_ERROR = 0x0002;
        private const int PBST_PAUSED = 0x0003;

        [Category("Behavior")]
        [Description("Event raised when the state of the Control is changed.")]
        public event StateChangedHandler StateChanged;

        [Category("Behavior")]
        [Description("This property allows the user to set the state of the ProgressBar.")]
        [DefaultValue(vState.Normal)]
        public vState State
        {
            get
            {
                if (Environment.OSVersion.Version.Major < 6)
                    return vState.Normal;
                if (this.Style == System.Windows.Forms.ProgressBarStyle.Blocks) return _State;
                else return vState.Normal;
            }
            set
            {
                _State = value;
                if (this.Style == System.Windows.Forms.ProgressBarStyle.Blocks)
                    ChangeState(_State);
            }
        }
        private void ChangeState(vState State)
        {
            if (Environment.OSVersion.Version.Major > 5)
            {
                SendMessage(this.Handle, PBM_SETSTATE, PBST_NORMAL, 0);

                switch (State)
                {
                    case vState.Pause:
                        SendMessage(this.Handle, PBM_SETSTATE, PBST_PAUSED, 0);
                        break;
                    case vState.Error:
                        SendMessage(this.Handle, PBM_SETSTATE, PBST_ERROR, 0);
                        break;
                    default:
                        SendMessage(this.Handle, PBM_SETSTATE, PBST_NORMAL, 0);
                        break;
                }
                if (StateChanged != null)
                    StateChanged(this, State);
            }
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 15)
                ChangeState(_State);
            base.WndProc(ref m);
        }

        [Category("Appearance")]
        [Description("This is the text you want to print to ProgressBar.")]
        [DefaultValue("")]
        public override string Text { get; set; }
        [Category("Appearance")]
        [Description("This is the text you want to print to ProgressBar.")]
        public override Font Font { get => base.Font; set => base.Font = value; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var mSize = e.Graphics.MeasureString(this.Text, this.Font);

            var mPos = new PointF((this.Height - mSize.Height) / 2, (this.Width - mSize.Width) / 2);

            e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), mPos);

        }
    }
}
