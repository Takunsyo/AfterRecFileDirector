using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RVMCore.Forms
{
    public partial class Uploader : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Uploader));
            this.pbAll = new RVMCore.Forms.ProgressBarKai();
            this.upLoadList = new System.Windows.Forms.ListBox();
            this.pbCurrent = new RVMCore.Forms.ProgressBarKai();
            this.lStatus = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btn_Open = new System.Windows.Forms.Button();
            this.btn_StartPause = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pbAll
            // 
            this.pbAll.Location = new System.Drawing.Point(12, 106);
            this.pbAll.Name = "pbAll";
            this.pbAll.Size = new System.Drawing.Size(461, 23);
            this.pbAll.TabIndex = 0;
            // 
            // upLoadList
            // 
            this.upLoadList.FormattingEnabled = true;
            this.upLoadList.ItemHeight = 12;
            this.upLoadList.Location = new System.Drawing.Point(12, 12);
            this.upLoadList.Name = "upLoadList";
            this.upLoadList.Size = new System.Drawing.Size(461, 88);
            this.upLoadList.TabIndex = 1;
            // 
            // pbCurrent
            // 
            this.pbCurrent.Location = new System.Drawing.Point(12, 135);
            this.pbCurrent.Name = "pbCurrent";
            this.pbCurrent.Size = new System.Drawing.Size(461, 23);
            this.pbCurrent.TabIndex = 2;
            // 
            // lStatus
            // 
            this.lStatus.Location = new System.Drawing.Point(12, 165);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(461, 40);
            this.lStatus.TabIndex = 3;
            this.lStatus.Text = "Status";
            this.lStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(398, 208);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btn_Open
            // 
            this.btn_Open.Location = new System.Drawing.Point(14, 206);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(75, 23);
            this.btn_Open.TabIndex = 5;
            this.btn_Open.Text = "Open Files";
            this.btn_Open.UseVisualStyleBackColor = true;
            // 
            // btn_StartPause
            // 
            this.btn_StartPause.Location = new System.Drawing.Point(317, 208);
            this.btn_StartPause.Name = "btn_StartPause";
            this.btn_StartPause.Size = new System.Drawing.Size(75, 23);
            this.btn_StartPause.TabIndex = 6;
            this.btn_StartPause.Text = "Start";
            this.btn_StartPause.UseVisualStyleBackColor = true;
            // 
            // Uploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(485, 241);
            this.Controls.Add(this.btn_StartPause);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.pbCurrent);
            this.Controls.Add(this.upLoadList);
            this.Controls.Add(this.pbAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Uploader";
            this.Text = "Uploader";
            this.Load += new System.EventHandler(this.Uploader_Load);
            this.ResumeLayout(false);

        }
        private ProgressBarKai pbAll;
        private ListBox upLoadList;
        private ProgressBarKai pbCurrent;
        private Label lStatus;
        private Button btnCancel;
        #endregion
        
        private GoogleWarpper.GoogleDrive mService;
        private List<UploadFiles> mFiles;
        private Button btn_Open;
        private Button btn_StartPause;
        private DateTime tmpTime;
        public Uploader(GoogleWarpper.GoogleDrive serv)
        {
            InitializeComponent();
            this.mService = serv;
            mFiles = new List<UploadFiles>();
        }

        private void Uploader_Load(object sender, EventArgs e)
        {
            this.pbCurrent.Font = this.Font;
            mService.UpdateProgressChanged += new RVMCore.GoogleWarpper.UpdateProgress((long value, long maxmum, int y) => {
                if (this.pbCurrent.InvokeRequired)
                {
                    Action act = new Action(() => {
                        if (value >= 0 && maxmum > 0)
                        {
                            this.pbCurrent.Style = ProgressBarStyle.Continuous;
                            this.pbCurrent.Maximum = (maxmum > int.MaxValue) ? (int)(maxmum / 2) : (int)maxmum;
                            this.pbCurrent.Value = (maxmum > int.MaxValue) ? (int)(value / 2) : (int)value;
                            this.pbCurrent.Text= "[" + getSizeString(value) + "/" + getSizeString(maxmum) + "]" + getSizeString(y) + "/s";
                        }
                    });
                    this.pbCurrent.Invoke(act);
                }
                if (this.lStatus.InvokeRequired)
                {
                    Action act = new Action(() => {
                        this.lStatus.Text = "[" + getSizeString(value) + "/" + getSizeString(maxmum) + "]" + getSizeString(y) + "/s";
                    });
                    this.lStatus.Invoke(act);
                }
            });
        }

        [JsonObject]
        private struct UploadFiles
        {
            public string Name;
            public string FullPath;
            public bool isUploaded;
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
}
