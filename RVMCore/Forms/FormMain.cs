using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RVMCore.DirectShowLib;

namespace RVMCore
{
    public class MainWindow : System.Windows.Forms.Form
    {
        #region "Design"
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private System.ComponentModel.IContainer components;

        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.tvList = new System.Windows.Forms.TreeView();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Screen = new System.Windows.Forms.PictureBox();
            this.lbLength = new System.Windows.Forms.Label();
            this.lbPosNow = new System.Windows.Forms.Label();
            this.tbProgress = new System.Windows.Forms.TrackBar();
            this.btnAbout = new System.Windows.Forms.Button();
            this.FileInfo = new System.Windows.Forms.TextBox();
            this.btnSetting = new System.Windows.Forms.Button();
            this.Infomation = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPP = new System.Windows.Forms.Button();
            this.tbVolume = new System.Windows.Forms.TrackBar();
            this.tvListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenWithDefaultPlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.CutVideo = new System.Windows.Forms.ToolStripMenuItem();
            this.ti = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Screen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbProgress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).BeginInit();
            this.tvListMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainer
            // 
            this.SplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainer.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.tvList);
            this.SplitContainer.Panel1MinSize = 100;
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.SplitContainer1);
            this.SplitContainer.Size = new System.Drawing.Size(862, 588);
            this.SplitContainer.SplitterDistance = 120;
            this.SplitContainer.TabIndex = 0;
            // 
            // tvList
            // 
            this.tvList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvList.Location = new System.Drawing.Point(0, 0);
            this.tvList.Name = "tvList";
            this.tvList.Size = new System.Drawing.Size(118, 586);
            this.tvList.TabIndex = 0;
            this.tvList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvList_AfterSelect);
            this.tvList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvList_NodeMouseClick);
            this.tvList.DoubleClick += new System.EventHandler(this.tvList_DoubleClick);
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.SplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer1.Name = "SplitContainer1";
            this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.Screen);
            this.SplitContainer1.Panel1.Controls.Add(this.lbLength);
            this.SplitContainer1.Panel1.Controls.Add(this.lbPosNow);
            this.SplitContainer1.Panel1.Controls.Add(this.tbProgress);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.btnAbout);
            this.SplitContainer1.Panel2.Controls.Add(this.FileInfo);
            this.SplitContainer1.Panel2.Controls.Add(this.btnSetting);
            this.SplitContainer1.Panel2.Controls.Add(this.Infomation);
            this.SplitContainer1.Panel2.Controls.Add(this.btnStop);
            this.SplitContainer1.Panel2.Controls.Add(this.btnPP);
            this.SplitContainer1.Panel2.Controls.Add(this.tbVolume);
            this.SplitContainer1.Panel2MinSize = 100;
            this.SplitContainer1.Size = new System.Drawing.Size(738, 588);
            this.SplitContainer1.SplitterDistance = 484;
            this.SplitContainer1.TabIndex = 10;
            // 
            // Screen
            // 
            this.Screen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Screen.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Screen.Location = new System.Drawing.Point(0, 0);
            this.Screen.Name = "Screen";
            this.Screen.Size = new System.Drawing.Size(736, 455);
            this.Screen.TabIndex = 0;
            this.Screen.TabStop = false;
            this.Screen.SizeChanged += new System.EventHandler(this.Screen_SizeChanged);
            this.Screen.DoubleClick += new System.EventHandler(this.Screen_Click);
            // 
            // lbLength
            // 
            this.lbLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbLength.AutoSize = true;
            this.lbLength.Location = new System.Drawing.Point(5, 467);
            this.lbLength.Name = "lbLength";
            this.lbLength.Size = new System.Drawing.Size(65, 12);
            this.lbLength.TabIndex = 9;
            this.lbLength.Text = "00:00:00.000";
            // 
            // lbPosNow
            // 
            this.lbPosNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPosNow.AutoSize = true;
            this.lbPosNow.Location = new System.Drawing.Point(5, 455);
            this.lbPosNow.Name = "lbPosNow";
            this.lbPosNow.Size = new System.Drawing.Size(65, 12);
            this.lbPosNow.TabIndex = 8;
            this.lbPosNow.Text = "00:00:00.000";
            // 
            // tbProgress
            // 
            this.tbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProgress.Location = new System.Drawing.Point(68, 455);
            this.tbProgress.Maximum = 1000;
            this.tbProgress.Name = "tbProgress";
            this.tbProgress.Size = new System.Drawing.Size(670, 45);
            this.tbProgress.TabIndex = 1;
            this.tbProgress.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbProgress.ValueChanged += new System.EventHandler(this.tbProgress_ValueChanged);
            this.tbProgress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbProgress_MouseDown);
            this.tbProgress.MouseLeave += new System.EventHandler(this.tbProgress_MouseLeave);
            this.tbProgress.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbProgress_MouseUp);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.Location = new System.Drawing.Point(646, 72);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(48, 23);
            this.btnAbout.TabIndex = 8;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // FileInfo
            // 
            this.FileInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileInfo.Location = new System.Drawing.Point(3, 6);
            this.FileInfo.Name = "FileInfo";
            this.FileInfo.Size = new System.Drawing.Size(574, 19);
            this.FileInfo.TabIndex = 7;
            // 
            // btnSetting
            // 
            this.btnSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetting.Location = new System.Drawing.Point(583, 72);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(63, 23);
            this.btnSetting.TabIndex = 6;
            this.btnSetting.Text = "Setting";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // Infomation
            // 
            this.Infomation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Infomation.Location = new System.Drawing.Point(3, 31);
            this.Infomation.Multiline = true;
            this.Infomation.Name = "Infomation";
            this.Infomation.Size = new System.Drawing.Size(574, 64);
            this.Infomation.TabIndex = 3;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(583, 36);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(106, 23);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // btnPP
            // 
            this.btnPP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPP.Location = new System.Drawing.Point(583, 6);
            this.btnPP.Name = "btnPP";
            this.btnPP.Size = new System.Drawing.Size(106, 23);
            this.btnPP.TabIndex = 4;
            this.btnPP.Text = "Play/Pause";
            this.btnPP.UseVisualStyleBackColor = true;
            this.btnPP.Click += new System.EventHandler(this.btnPP_Click);
            // 
            // tbVolume
            // 
            this.tbVolume.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbVolume.Location = new System.Drawing.Point(700, 6);
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbVolume.Size = new System.Drawing.Size(45, 89);
            this.tbVolume.TabIndex = 2;
            this.tbVolume.Value = 10;
            this.tbVolume.Scroll += new System.EventHandler(this.tbVolume_Scroll);
            // 
            // tvListMenu
            // 
            this.tvListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenWithDefaultPlayer,
            this.OpenFolder,
            this.CutVideo});
            this.tvListMenu.Name = "ContextMenuStrip1";
            this.tvListMenu.Size = new System.Drawing.Size(208, 70);
            // 
            // OpenWithDefaultPlayer
            // 
            this.OpenWithDefaultPlayer.Name = "OpenWithDefaultPlayer";
            this.OpenWithDefaultPlayer.Size = new System.Drawing.Size(207, 22);
            this.OpenWithDefaultPlayer.Text = "&Open with Default player.";
            this.OpenWithDefaultPlayer.Click += new System.EventHandler(this.OpenWithDefaultPlayer_Click);
            // 
            // OpenFolder
            // 
            this.OpenFolder.Name = "OpenFolder";
            this.OpenFolder.Size = new System.Drawing.Size(207, 22);
            this.OpenFolder.Text = "Open &Folder in Explorer.";
            this.OpenFolder.Click += new System.EventHandler(this.OpenFolder_Click);
            // 
            // CutVideo
            // 
            this.CutVideo.Name = "CutVideo";
            this.CutVideo.Size = new System.Drawing.Size(207, 22);
            this.CutVideo.Text = "&Cut Video";
            this.CutVideo.Click += new System.EventHandler(this.CutVideo_Click);
            // 
            // ti
            // 
            this.ti.Tick += new System.EventHandler(this.ti_sub);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 588);
            this.Controls.Add(this.SplitContainer);
            this.Name = "MainWindow";
            this.Text = "RecPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_Closing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel1.PerformLayout();
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Screen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbProgress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVolume)).EndInit();
            this.tvListMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            Mainplayer.Stop();
        }

        private SplitContainer SplitContainer;
        private TreeView tvList;
        private TextBox Infomation;
        private TrackBar tbVolume;
        private TrackBar tbProgress;
        private PictureBox Screen;
        private Button btnSetting;
        private Button btnStop;
        private Button btnPP;
        private TextBox FileInfo;
        private ContextMenuStrip tvListMenu;
        private ToolStripMenuItem OpenWithDefaultPlayer;
        private ToolStripMenuItem OpenFolder;
        private Label lbLength;
        private Label lbPosNow;
        private SplitContainer SplitContainer1;
        private Button btnAbout;
        private ToolStripMenuItem CutVideo;
        private Timer ti;
        #endregion
        
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private MediaPlayer Mainplayer;
        public SettingObj mySetting = SettingObj.Read();

        private string SelectedFilePath = string.Empty;

# region "Icons"

        private const Int32 MAX_PATH = 260;
        private const Int32 SHGFI_ICON = 0x100;
        private const Int32 SHGFI_USEFILEATTRIBUTES = 0x10;
        private const Int32 FILE_ATTRIBUTE_NORMAL = 0x80;

        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public Int32 iIcon;
            public Int32 dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private enum IconSize
        {
            SHGFI_LARGEICON = 0,
            SHGFI_SMALLICON = 1
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, Int32 dwFileAttributes, ref SHFILEINFO psfi, Int32 cbFileInfo, Int32 uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // get associated icon (as bitmap).
        private Bitmap GetFileIcon(string fileExt, IconSize ICOsize = IconSize.SHGFI_SMALLICON)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            shinfo.szDisplayName = new string(Strings.Chr(0), MAX_PATH);
            shinfo.szTypeName = new string(Strings.Chr(0), 80);
            SHGetFileInfo(fileExt, FILE_ATTRIBUTE_NORMAL, ref shinfo, Marshal.SizeOf(shinfo), SHGFI_ICON | (int)ICOsize | SHGFI_USEFILEATTRIBUTES);
            Bitmap bmp = System.Drawing.Icon.FromHandle(shinfo.hIcon).ToBitmap();
            DestroyIcon(shinfo.hIcon); // must destroy icon to avoid GDI leak!
            return bmp; // return icon as a bitmap
        }
# endregion

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Mainplayer = new MediaPlayer(Screen.Handle, Screen.Size);//, this.Handle
            tvList.ImageList = new ImageList();
            tvList.ImageList.Images.Add(GetFileIcon(AppDomain.CurrentDomain.BaseDirectory));
            tvList.ImageList.Images.Add(GetFileIcon(".ts"));
            tvList.ContextMenuStrip = tvListMenu;
            foreach (var s in System.IO.Directory.GetDirectories(mySetting.StorageFolder))
            {
                TreeNode item = new TreeNode();
                {
                    var withBlock = item;
                    withBlock.Text = s.Substring(s.LastIndexOf(@"\") + 1);
                    withBlock.Tag = s;
                    withBlock.ImageIndex = 0;
                }
                tvList.Nodes.Add(item);
            }
            foreach (var s in System.IO.Directory.GetFiles(mySetting.StorageFolder))
            {
                if (System.IO.Path.GetExtension(s).ToLower() == ".ts")
                {
                    TreeNode item = new TreeNode();
                    {
                        var withBlock = item;
                        withBlock.Text = s.Substring(s.LastIndexOf(@"\") + 1);
                        withBlock.Tag = s;
                        withBlock.ImageIndex = 1;
                    }
                    tvList.Nodes.Add(item);
                }
            }
        }


        private void btnPP_Click(object sender, EventArgs e)
        {
            if (Mainplayer.State.HasFlag(PlayState.Closed))
            {
                if (!string.IsNullOrWhiteSpace(SelectedFilePath) & System.IO.File.Exists(SelectedFilePath))
                {
                    Mainplayer.Play(SelectedFilePath); ti.Enabled = true;
                }
                clicked = true;
                return;
            }
            if (Mainplayer.State.HasFlag(PlayState.Playing))
                Mainplayer.Pause();
            else
            {
                Mainplayer.Play();
                ti.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (Mainplayer.State.HasFlag(PlayState.Opened))
            {
                Mainplayer.Stop();
                ti.Enabled = false;
                lbPosNow.Text = "00:00:00";
                lbLength.Text = "00:00:00";
            }
        }

        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            Mainplayer.Volume = tbVolume.Value * 10;
        }

        private void Screen_SizeChanged(object sender, EventArgs e)
        {
            if (Mainplayer == null)
                return;
            Mainplayer.ScreenSize = Screen.Size;
        }

        private void MainWindow_Closing(object sender, FormClosingEventArgs e)
        {
            //Mainplayer.Dispose();
        }
        private bool mouseOn = false;


        private void tbProgress_MouseDown(object sender, MouseEventArgs e)
        {
            mouseOn = true;
        }

        private void tbProgress_MouseLeave(object sender, EventArgs e)
        {
            mouseOn = false;
        }

        private void tbProgress_MouseUp(object sender, MouseEventArgs e)
        {
            mouseOn = false;
        }

        private void tbProgress_ValueChanged(object sender, EventArgs e)
        {
            if (mouseOn && !(Mainplayer is null))
            {
                int max = Mainplayer.Length;
                int val = max / tbProgress.Maximum * tbProgress.Value;
                Mainplayer.Position = val;
            }
        }

        private void ti_sub(object sender, EventArgs e)
        {
            if (Mainplayer.State.HasFlag(PlayState.Opened))
            {
                int hh, mm, ss, ms;
                var pos = Mainplayer.Position;
                hh = pos / 3600000;
                mm = (pos % 3600000) / 60000;
                ss = (pos % 60000) / 1000;
                ms = pos % 1000;
                lbPosNow.Text = string.Format("{0}:{1}:{2}.{3}", hh.ToString("D2"), mm.ToString("D2"), ss.ToString("D2"), ms.ToString("D3"));
                hh = (Mainplayer.Length - pos) / 3600000;
                mm = ((Mainplayer.Length - pos) % 3600000) / 60000;
                ss = ((Mainplayer.Length - pos) % 60000) / 1000;
                ms = (Mainplayer.Length - pos) % 1000;
                lbLength.Text = string.Format("{0}:{1}:{2}.{3}", hh.ToString("D2"), mm.ToString("D2"), ss.ToString("D2"), ms.ToString("D3"));
                if (!mouseOn)
                    tbProgress.Value = pos / Mainplayer.Length * tbProgress.Maximum;
            }
        }

        internal class tbEventArgs : EventArgs
        {
            private bool isPlayerSetting = false;
            public tbEventArgs(bool yes)
            {
                isPlayerSetting = yes;
            }
        }

        private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            System.IO.FileAttributes attr = System.IO.File.GetAttributes((string)tvList.SelectedNode.Tag);
            if (attr.HasFlag(System.IO.FileAttributes.Directory))
            {
                tvList.SelectedNode.Nodes.Clear();
                foreach (var s in System.IO.Directory.GetDirectories((string)tvList.SelectedNode.Tag))
                {
                    TreeNode item = new TreeNode();
                    {
                        var withBlock = item;
                        withBlock.Text = s.Substring(s.LastIndexOf(@"\") + 1);
                        withBlock.Tag = s;
                        withBlock.ImageIndex = 0;
                    }
                    tvList.SelectedNode.Nodes.Add(item);
                }
                foreach (var s in System.IO.Directory.GetFiles((string)tvList.SelectedNode.Tag))
                {
                    if (System.IO.Path.GetExtension(s).ToLower() == ".ts")
                    {
                        TreeNode item = new TreeNode();
                        {
                            var withBlock = item;
                            withBlock.BackColor = Color.Violet;
                            withBlock.Text = s.Substring(s.LastIndexOf(@"\") + 1);
                            withBlock.Tag = s;
                            withBlock.ImageIndex = 1;
                        }
                        tvList.SelectedNode.Nodes.Add(item);
                    }
                }
            }
            else
            {
                SelectedFilePath = (string)tvList.SelectedNode.Tag;
                if (System.IO.File.Exists(SelectedFilePath + ".xml"))
                {
                    var myObj = StreamFile.FromXml(SelectedFilePath + ".xml");
                    Infomation.Text = myObj.Infomation;
                    FileInfo.Text = myObj.Title;
                }
                else
                {
                    Infomation.Text = (string)tvList.SelectedNode.Tag;
                    FileInfo.Text = "Unknow.";
                }
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            using (Settings mSetting = new Settings())
            {
                if (mSetting.ShowDialog() == DialogResult.OK)
                    mySetting = SettingObj.Read();
            }
        }

        private void tvList_DoubleClick(object sender, EventArgs e)
        {
            if (tvList.SelectedNode == null)
                return;
            System.IO.FileAttributes attr = System.IO.File.GetAttributes((string)tvList.SelectedNode.Tag);
            if (!attr.HasFlag(System.IO.FileAttributes.Directory) && System.IO.Path.GetExtension((string)tvList.SelectedNode.Tag).ToLower() == ".ts")
            {
                if (Mainplayer.MediaFilePath.Equals((string)tvList.SelectedNode.Tag))
                {
                    Mainplayer.Play(); return;
                }
                if (Mainplayer.State.HasFlag(PlayState.Opened))
                {
                    Mainplayer.Stop();
                    ti.Enabled = false;
                }
                Mainplayer.Play((string)tvList.SelectedNode.Tag);
                ti.Enabled = true;
            }
        }

        private void tvList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tvList.SelectedNode = e.Node;
                System.IO.FileAttributes attr = System.IO.File.GetAttributes((string)tvList.SelectedNode.Tag);
                if (attr.HasFlag(System.IO.FileAttributes.Directory))
                {
                    OpenWithDefaultPlayer.Visible = false;
                    CutVideo.Visible = false;
                }
                else
                {
                    OpenWithDefaultPlayer.Visible = true;
                    CutVideo.Visible = true;
                }
                tvList.ContextMenuStrip.Show();
            }
        }

        private void OpenFolder_Click(object sender, EventArgs e)
        {
            System.IO.FileAttributes attr = System.IO.File.GetAttributes((string)tvList.SelectedNode.Tag);
            if (attr.HasFlag(System.IO.FileAttributes.Directory))
                Process.Start("explorer.exe", string.Format("\"{0}\"", tvList.SelectedNode.Tag));
            else
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", tvList.SelectedNode.Tag));
        }

        private void OpenWithDefaultPlayer_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start((string)tvList.SelectedNode.Tag);
        }

        private bool clicked = false;

        private void btnAbout_Click(object sender, EventArgs e)
        {
            //AboutBox tmp = new AboutBox();
            //tmp.Show();
        }

        private void CutVideo_Click(object sender, EventArgs e)
        {
            //VideoCutter myItem = new VideoCutter(tvList.SelectedNode.Tag);
            //myItem.Show();
        }

        private void Screen_Click(object sender, EventArgs e)
        {
            if (this.Mainplayer.State.HasFlag(PlayState.Opened))
            {
                this.Mainplayer.FullScreen = !this.Mainplayer.FullScreen;
            }
        }

    }
}
