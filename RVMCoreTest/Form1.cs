using RVMCore.EPGStationWarpper;
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
            this.MaximizeBox = false;

            updateList();
        }
        RVMCore.GoogleWarpper.GoogleDrive mService = new RVMCore.GoogleWarpper.GoogleDrive();

        //Only for upload
        List<UploadFile> fList = new List<UploadFile>();
        System.Threading.Thread mThread;
        BindingSource listBox1Soucrce = new BindingSource();
        Stopwatch TimeOutWatch = new Stopwatch();
        private void button1_Click(object sender, EventArgs e)
        {
            using (var mdlg = new OpenFileDialog())
            {
                mdlg.CheckFileExists = true;
                mdlg.Filter= "TransPortStream|*.ts";
                mdlg.InitialDirectory = @"E:\1[アニメ類]\";
                mdlg.Multiselect = true;
                if (mdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach(string i in mdlg.FileNames)
                    {
                        if(!fList.Any(x=>x==i))fList.Add(i);
                    }
                }
                else return;
            }

            mService.MaxBytesPerSecond = 650 * 1024;
            //var exi = mService.RemoteFileExists(local, remot);
            //MessageBox.Show(exi.ToString());
            System.Threading.ThreadStart work = new System.Threading.ThreadStart(() =>
            {
                do
                {
                    UploadFile local = null;
                    try
                    {
                        local = fList.First(x => !x.IsOver);
                    }
                    catch
                    {
                        return;
                    }
                    int index = fList.FindIndex(x=> x==local);
                    if (local == null) return;
                    string remot = local.RemotePath;
                    this.Invoke(new Action(() =>
                    {
                        this.Text = "[Uploading]" + local.FileName;
                        this.textBox1.Text = local;
                        this.textBox1.ReadOnly = true;
                        //button1.Enabled = false;
                        this.progressBar1.Text = "Initialize upload (MD5 File Checking...)";
                        this.progressBar1.Value = 0;
                        this.progressBar1.Style = ProgressBarStyle.Marquee;
                    }));
                    updateList();
                    //listBox1Soucrce.ResetBindings(false);
                    bool isSuccess = false;
                    if (!System.IO.File.Exists(RVMCore.GoogleWarpper.GoogleDrive.GetUploadStatusPath(local)))
                    {
                        if (!mService.RemoteFileExists(local, remot))
                        {
                            
                            fList[index] = local;
                            isSuccess=mService.UploadResumable(local, remot) !=null;
                            this.progressBar1.Invoke(new Action(() => { this.progressBar1.Text = "Uploading..."; }));
                        }
                        else
                        {
                            isSuccess = true;
                        }
                    }
                    else isSuccess= mService.UploadResumable(local, remot) !=null;
                    //button1.Invoke(new Action(() => { button1.Enabled = true; }));
                    this.progressBar1.Invoke(new Action(() => {
                        this.progressBar1.Text = "Done.";
                        this.progressBar1.Style = ProgressBarStyle.Blocks;
                    }));
                    local.IsOver = isSuccess;
                    fList[index] = local;
                    updateList();
                    //listBox1Soucrce.ResetBindings(false);
                } while (true);
            });

            if (mThread == null || !mThread.IsAlive) { mThread = new System.Threading.Thread(work); mThread.Start(); }
            updateList();
            //listBox1Soucrce.ResetBindings(false);
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

        private async void Form1_Load(object sender, EventArgs e)
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
                            //var mSize = this.progressBar1.CreateGraphics().MeasureString(mstr, this.Font);
                            //var mPos = new PointF((this.progressBar1.Height - mSize.Height) / 2, (this.progressBar1.Width - mSize.Width) / 2);
                            //this.progressBar1.CreateGraphics().DrawString(mstr, this.Font, new SolidBrush(this.ForeColor), mPos);
                            this.progressBar1.Text = mstr;
                        }
                    });
                    this.progressBar1.Invoke(act);
                }
                //if (this.lStatus.InvokeRequired)
                //{
                //    Action act = new Action(() => {
                //        this.lStatus.Text = "["+ getSizeString(value) +"/" +getSizeString(maxmum)+"]" + getSizeString(y)+"/s";
                //    });
                //    this.lStatus.Invoke(act);
                //}
                //updateList();
                ////listBox1Soucrce.ResetBindings(false);
            });
            this.updateList();


            //var mList = mService.GetGoogleFiles("name contains '.ts' and mimeType='text/plain'");

            //var change =new Action<Google.Apis.Drive.v3.Data.File>(x =>{
            //    var n = new Google.Apis.Drive.v3.Data.File();
            //    n.Name = x.Name;
            //    n.MimeType = "video/mp2t";
            //    var y = mService.UpdateFile(x.Id, n);
            //    if(!(y is null))
            //    {
            //        MessageBox.Show(string.Format("File '{0}' is updated \n MimeType is '{1}'", y.Name, y.MimeType));
            //    }
            //});
            ////mList.ForEach(change);
            //var mID = "1NaeKDxNMHr59NOFppnjTwLMI5Kt1hn5i";

            //await mService.DownloadResumableAsync(mID, "D:\\",true);
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

        private void updateList()
        {
            if (this.listBox1.InvokeRequired)
            {
                this.listBox1.Invoke(new Action(() => {
                    this.listBox1Soucrce = new BindingSource();
                    this.listBox1Soucrce.DataSource = this.fList;
                    this.listBox1.DataSource = listBox1Soucrce;
                    this.listBox1.DisplayMember = "ShowName";
                    this.listBox1.ValueMember = "FullPath";
                }));
            }
            else
            {
                this.listBox1Soucrce = new BindingSource();
                this.listBox1Soucrce.DataSource = this.fList;
                this.listBox1.DataSource = listBox1Soucrce;
                this.listBox1.DisplayMember = "ShowName";
                this.listBox1.ValueMember = "FullPath";
            }
        }
    }

    public class UploadFile:INotifyPropertyChanged
    {
        public string ShowName {
            get {
                if (this.IsOver) return "==" + this.FileName;
                else if (this.IsUploading) return "->" + this.FileName;
                else return "--" + this.FileName;
            }
        }
        public string FileName { get { return System.IO.Path.GetFileName(this.FullPath); } }
        public string FullPath { get; }
        public bool IsUploading { get { return System.IO.File.Exists(RVMCore.GoogleWarpper.GoogleDrive.GetUploadStatusPath(this.FullPath)); } }
        private bool _IsOVer;
        public bool IsOver {
            get {
                return _IsOVer;
            }
            set {
                this._IsOVer = value;
                this.NotifyPropertyChanged("ShowName");
            } }
        public string RemotePath {
            get
            {
                return System.IO.Path.GetDirectoryName(this.FullPath).Replace(System.IO.Path.GetPathRoot(this.FullPath), @"\EPGRecords\");
            }
        }
        public UploadFile(string filePath)
        {
            FullPath = filePath;
            IsOver = false;
        }

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static IEnumerable<UploadFile> UploadFiles(IEnumerable<string> filePathes)
        {
            var tmp = new List<UploadFile>();
            foreach (string i in filePathes)
            {
                tmp.Add(new UploadFile(i));
            }
            return tmp;
        }

        public static implicit operator UploadFile(string input)
        {
            return new UploadFile(input);
        }

        public static implicit operator string(UploadFile input)
        {
            return input.FullPath;
        }

        public static bool operator ==(UploadFile obj1 , UploadFile obj2 ){
            if (obj1 is null||obj2 is null) return false;
            return obj1.FullPath == obj2.FullPath;
        }
        public static bool operator !=(UploadFile obj1, UploadFile obj2)
        {
            if (obj1 is null || obj2 is null) return true;
            return obj1.FullPath != obj2.FullPath;
        }

        public override int GetHashCode()
        {
            return this.FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null ) return false;
            return this==((UploadFile)obj);
        }
        //public static implicit operator List<UploadFile>(string[] input)
        //{
        //    foreach (string i in input)
        //    {
        //        yield return new UploadFile(i);
        //    }
        //}
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

        public VistaProgressBar(): base()
        {
            this.ForeColor = Color.Black;
            //SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

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
            if(m.Msg == 15)
            {
                using(var g = Graphics.FromHwnd(Handle))
                {
                    using(var sfm = new StringFormat())
                    {
                        sfm.Alignment = StringAlignment.Center;
                        sfm.LineAlignment = StringAlignment.Center;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        g.DrawString(this.Text,
                            this.Font,
                            new SolidBrush(Color.FromArgb(256 / 3, Color.WhiteSmoke)),
                            new RectangleF(1, 1, Width, Height),
                            sfm
                            );
                        g.DrawString(this.Text, 
                            this.Font, 
                            new SolidBrush(Color.Black), 
                            new RectangleF(0, 0, Width, Height), sfm);
                    }

                    //TextRenderer.DrawText(g,
                    //   this.Text,
                    //   this.Font,
                    //   new Rectangle(1, 1, Width, Height),Color.FromArgb(256/3,Color.WhiteSmoke) ,
                    //   TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);

                    //TextRenderer.DrawText(g,
                    //   this.Text,
                    //   this.Font,
                    //   new Rectangle(0, 0, Width, Height), this.ForeColor,
                    //   TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis );

                }
            }
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

            //var mSize = e.Graphics.MeasureString(this.Text, this.Font);

            //var mPos = new PointF((this.Height - mSize.Height) / 2, (this.Width - mSize.Width) / 2);

            //e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), mPos);
            TextRenderer.DrawText(e.Graphics, 
                this.Text, 
                this.Font, 
                new Rectangle(0, 0, Width, Height), this.ForeColor, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);

        }
    }
}
