using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RVMCore
{
    class Settings : System.Windows.Forms.Form
    {
#region"Design"
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
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.TableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbBeep = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.TableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tbFolder = new System.Windows.Forms.TextBox();
            this.Button1 = new System.Windows.Forms.Button();
            this.cbARR = new System.Windows.Forms.CheckBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.TextBox3 = new System.Windows.Forms.TextBox();
            this.TextBox4 = new System.Windows.Forms.TextBox();
            this.TextBox5 = new System.Windows.Forms.TextBox();
            this.TextBox6 = new System.Windows.Forms.TextBox();
            this.TextBox7 = new System.Windows.Forms.TextBox();
            this.TextBox8 = new System.Windows.Forms.TextBox();
            this.TextBox9 = new System.Windows.Forms.TextBox();
            this.TextBox10 = new System.Windows.Forms.TextBox();
            this.TextBox11 = new System.Windows.Forms.TextBox();
            this.TextBox12 = new System.Windows.Forms.TextBox();
            this.TableLayoutPanel1.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.TableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(357, 426);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 27);
            this.TableLayoutPanel1.TabIndex = 0;
            // 
            // OK_Button
            // 
            this.OK_Button.AllowDrop = true;
            this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OK_Button.Location = new System.Drawing.Point(3, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 21);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = "确定";
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(67, 21);
            this.Cancel_Button.TabIndex = 1;
            this.Cancel_Button.Text = "取消";
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // TableLayoutPanel2
            // 
            this.TableLayoutPanel2.ColumnCount = 2;
            this.TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel2.Controls.Add(this.cbBeep, 0, 1);
            this.TableLayoutPanel2.Controls.Add(this.Label1, 0, 0);
            this.TableLayoutPanel2.Controls.Add(this.Label4, 0, 2);
            this.TableLayoutPanel2.Controls.Add(this.Label5, 0, 3);
            this.TableLayoutPanel2.Controls.Add(this.Label6, 0, 4);
            this.TableLayoutPanel2.Controls.Add(this.Label7, 0, 5);
            this.TableLayoutPanel2.Controls.Add(this.Label8, 0, 6);
            this.TableLayoutPanel2.Controls.Add(this.Label9, 0, 7);
            this.TableLayoutPanel2.Controls.Add(this.Label10, 0, 8);
            this.TableLayoutPanel2.Controls.Add(this.Label11, 0, 9);
            this.TableLayoutPanel2.Controls.Add(this.Label12, 0, 10);
            this.TableLayoutPanel2.Controls.Add(this.Label13, 0, 11);
            this.TableLayoutPanel2.Controls.Add(this.Label14, 0, 12);
            this.TableLayoutPanel2.Controls.Add(this.Label15, 0, 13);
            this.TableLayoutPanel2.Controls.Add(this.Label16, 0, 14);
            this.TableLayoutPanel2.Controls.Add(this.TableLayoutPanel3, 1, 0);
            this.TableLayoutPanel2.Controls.Add(this.cbARR, 1, 1);
            this.TableLayoutPanel2.Controls.Add(this.TextBox1, 1, 3);
            this.TableLayoutPanel2.Controls.Add(this.TextBox2, 1, 4);
            this.TableLayoutPanel2.Controls.Add(this.TextBox3, 1, 5);
            this.TableLayoutPanel2.Controls.Add(this.TextBox4, 1, 6);
            this.TableLayoutPanel2.Controls.Add(this.TextBox5, 1, 7);
            this.TableLayoutPanel2.Controls.Add(this.TextBox6, 1, 8);
            this.TableLayoutPanel2.Controls.Add(this.TextBox7, 1, 9);
            this.TableLayoutPanel2.Controls.Add(this.TextBox8, 1, 10);
            this.TableLayoutPanel2.Controls.Add(this.TextBox9, 1, 11);
            this.TableLayoutPanel2.Controls.Add(this.TextBox10, 1, 12);
            this.TableLayoutPanel2.Controls.Add(this.TextBox11, 1, 13);
            this.TableLayoutPanel2.Controls.Add(this.TextBox12, 1, 14);
            this.TableLayoutPanel2.Location = new System.Drawing.Point(12, 12);
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            this.TableLayoutPanel2.RowCount = 15;
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.TableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel2.Size = new System.Drawing.Size(491, 408);
            this.TableLayoutPanel2.TabIndex = 1;
            // 
            // cbBeep
            // 
            this.cbBeep.AutoSize = true;
            this.cbBeep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbBeep.Location = new System.Drawing.Point(3, 30);
            this.cbBeep.Name = "cbBeep";
            this.cbBeep.Size = new System.Drawing.Size(239, 21);
            this.cbBeep.TabIndex = 31;
            this.cbBeep.Text = "Allow beep after recoding";
            this.cbBeep.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.Location = new System.Drawing.Point(3, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(239, 27);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Record Folder Root";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.TableLayoutPanel2.SetColumnSpan(this.Label4, 2);
            this.Label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label4.Location = new System.Drawing.Point(3, 54);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(485, 27);
            this.Label4.TabIndex = 3;
            this.Label4.Text = "FolderNames:  enable to be banked.";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label5.Location = new System.Drawing.Point(3, 81);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(239, 27);
            this.Label5.TabIndex = 4;
            this.Label5.Text = "ニュース・報道";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label6.Location = new System.Drawing.Point(3, 108);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(239, 27);
            this.Label6.TabIndex = 5;
            this.Label6.Text = "スポーツ";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label7.Location = new System.Drawing.Point(3, 135);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(239, 27);
            this.Label7.TabIndex = 6;
            this.Label7.Text = "ドラマ";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label8.Location = new System.Drawing.Point(3, 162);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(239, 27);
            this.Label8.TabIndex = 7;
            this.Label8.Text = "音楽";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label9.Location = new System.Drawing.Point(3, 189);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(239, 27);
            this.Label9.TabIndex = 8;
            this.Label9.Text = "バラエティー";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label10.Location = new System.Drawing.Point(3, 216);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(239, 27);
            this.Label10.TabIndex = 9;
            this.Label10.Text = "映画";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label11.Location = new System.Drawing.Point(3, 243);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(239, 27);
            this.Label11.TabIndex = 10;
            this.Label11.Text = "アニメ・特撮";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label12.Location = new System.Drawing.Point(3, 270);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(239, 27);
            this.Label12.TabIndex = 11;
            this.Label12.Text = "情報・ワイドショー";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label13.Location = new System.Drawing.Point(3, 297);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(239, 27);
            this.Label13.TabIndex = 12;
            this.Label13.Text = "ドキュメンタリー";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label14.Location = new System.Drawing.Point(3, 324);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(239, 27);
            this.Label14.TabIndex = 13;
            this.Label14.Text = "劇場・公演";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label15.Location = new System.Drawing.Point(3, 351);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(239, 27);
            this.Label15.TabIndex = 14;
            this.Label15.Text = "趣味・教育";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label16.Location = new System.Drawing.Point(3, 378);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(239, 30);
            this.Label16.TabIndex = 15;
            this.Label16.Text = "その他";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TableLayoutPanel3
            // 
            this.TableLayoutPanel3.ColumnCount = 2;
            this.TableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77F));
            this.TableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.TableLayoutPanel3.Controls.Add(this.tbFolder, 0, 0);
            this.TableLayoutPanel3.Controls.Add(this.Button1, 1, 0);
            this.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel3.Location = new System.Drawing.Point(245, 0);
            this.TableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.TableLayoutPanel3.Name = "TableLayoutPanel3";
            this.TableLayoutPanel3.RowCount = 1;
            this.TableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel3.Size = new System.Drawing.Size(246, 27);
            this.TableLayoutPanel3.TabIndex = 16;
            // 
            // tbFolder
            // 
            this.tbFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFolder.Location = new System.Drawing.Point(3, 3);
            this.tbFolder.Name = "tbFolder";
            this.tbFolder.Size = new System.Drawing.Size(185, 19);
            this.tbFolder.TabIndex = 0;
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(194, 3);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(49, 21);
            this.Button1.TabIndex = 1;
            this.Button1.Text = "Brows";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // cbARR
            // 
            this.cbARR.AutoSize = true;
            this.cbARR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbARR.Location = new System.Drawing.Point(248, 30);
            this.cbARR.Name = "cbARR";
            this.cbARR.Size = new System.Drawing.Size(240, 21);
            this.cbARR.TabIndex = 17;
            this.cbARR.Text = "Allow Record on root folder";
            this.cbARR.UseVisualStyleBackColor = true;
            // 
            // TextBox1
            // 
            this.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox1.Location = new System.Drawing.Point(248, 84);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(240, 19);
            this.TextBox1.TabIndex = 19;
            // 
            // TextBox2
            // 
            this.TextBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox2.Location = new System.Drawing.Point(248, 111);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(240, 19);
            this.TextBox2.TabIndex = 20;
            // 
            // TextBox3
            // 
            this.TextBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox3.Location = new System.Drawing.Point(248, 138);
            this.TextBox3.Name = "TextBox3";
            this.TextBox3.Size = new System.Drawing.Size(240, 19);
            this.TextBox3.TabIndex = 21;
            // 
            // TextBox4
            // 
            this.TextBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox4.Location = new System.Drawing.Point(248, 165);
            this.TextBox4.Name = "TextBox4";
            this.TextBox4.Size = new System.Drawing.Size(240, 19);
            this.TextBox4.TabIndex = 22;
            // 
            // TextBox5
            // 
            this.TextBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox5.Location = new System.Drawing.Point(248, 192);
            this.TextBox5.Name = "TextBox5";
            this.TextBox5.Size = new System.Drawing.Size(240, 19);
            this.TextBox5.TabIndex = 23;
            // 
            // TextBox6
            // 
            this.TextBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox6.Location = new System.Drawing.Point(248, 219);
            this.TextBox6.Name = "TextBox6";
            this.TextBox6.Size = new System.Drawing.Size(240, 19);
            this.TextBox6.TabIndex = 24;
            // 
            // TextBox7
            // 
            this.TextBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox7.Location = new System.Drawing.Point(248, 246);
            this.TextBox7.Name = "TextBox7";
            this.TextBox7.Size = new System.Drawing.Size(240, 19);
            this.TextBox7.TabIndex = 25;
            // 
            // TextBox8
            // 
            this.TextBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox8.Location = new System.Drawing.Point(248, 273);
            this.TextBox8.Name = "TextBox8";
            this.TextBox8.Size = new System.Drawing.Size(240, 19);
            this.TextBox8.TabIndex = 26;
            // 
            // TextBox9
            // 
            this.TextBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox9.Location = new System.Drawing.Point(248, 300);
            this.TextBox9.Name = "TextBox9";
            this.TextBox9.Size = new System.Drawing.Size(240, 19);
            this.TextBox9.TabIndex = 27;
            // 
            // TextBox10
            // 
            this.TextBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox10.Location = new System.Drawing.Point(248, 327);
            this.TextBox10.Name = "TextBox10";
            this.TextBox10.Size = new System.Drawing.Size(240, 19);
            this.TextBox10.TabIndex = 28;
            // 
            // TextBox11
            // 
            this.TextBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox11.Location = new System.Drawing.Point(248, 354);
            this.TextBox11.Name = "TextBox11";
            this.TextBox11.Size = new System.Drawing.Size(240, 19);
            this.TextBox11.TabIndex = 29;
            // 
            // TextBox12
            // 
            this.TextBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox12.Location = new System.Drawing.Point(248, 381);
            this.TextBox12.Name = "TextBox12";
            this.TextBox12.Size = new System.Drawing.Size(240, 19);
            this.TextBox12.TabIndex = 30;
            // 
            // Settings
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 464);
            this.Controls.Add(this.TableLayoutPanel2);
            this.Controls.Add(this.TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel2.ResumeLayout(false);
            this.TableLayoutPanel2.PerformLayout();
            this.TableLayoutPanel3.ResumeLayout(false);
            this.TableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private TableLayoutPanel TableLayoutPanel2;
        private Label Label1;
        private Label Label4;
        private Label Label5;
        private Label Label6;
        private Label Label7;
        private Label Label8;
        private Label Label9;
        private Label Label10;
        private Label Label11;
        private Label Label12;
        private Label Label13;
        private Label Label14;
        private Label Label15;
        private Label Label16;
        private CheckBox cbBeep;
        private TableLayoutPanel TableLayoutPanel3;
        private TextBox tbFolder;
        private Button Button1;
        private CheckBox cbARR;
        private TextBox TextBox1;
        private TextBox TextBox2;
        private TextBox TextBox3;
        private TextBox TextBox4;
        private TextBox TextBox5;
        private TextBox TextBox6;
        private TextBox TextBox7;
        private TextBox TextBox8;
        private TextBox TextBox9;
        private TextBox TextBox10;
        private TextBox TextBox11;
        private TextBox TextBox12;
        #endregion
        private SettingObj mySetting;
        public Settings()
        {
            this.InitializeComponent();
        }

        private void SettingToForm()
        {
                tbFolder.Text = mySetting.StorageFolder;
                cbBeep.Checked = mySetting.AllowBeep;
                cbARR.Checked = mySetting.AllowStoreOnBaseFolderIfTagIsNull;
                TextBox1.Text = mySetting.FolderTag_News;
                TextBox2.Text = mySetting.FolderTag_Sports;
                TextBox3.Text = mySetting.FolderTag_Docum;
                TextBox4.Text = mySetting.FolderTag_Music;
                TextBox5.Text = mySetting.FolderTag_Variety;
                TextBox6.Text = mySetting.FolderTag_Movie;
                TextBox7.Text = mySetting.FolderTag_Anime;
                TextBox8.Text = mySetting.FolderTag_Info;
                TextBox9.Text = mySetting.FolderTag_Docum;
                TextBox10.Text = mySetting.FolderTag_Live;
                TextBox11.Text = mySetting.FolderTag_Edu;
                TextBox12.Text = mySetting.FolderTag_Other;
        }
        private void FormToSettings()
        {
                mySetting.StorageFolder = tbFolder.Text;
                mySetting.AllowBeep = cbBeep.Checked;
                mySetting.AllowStoreOnBaseFolderIfTagIsNull = cbARR.Checked;
                mySetting.FolderTag_News = TextBox1.Text;
                mySetting.FolderTag_Sports = TextBox2.Text;
                mySetting.FolderTag_Docum = TextBox3.Text;
                mySetting.FolderTag_Music = TextBox4.Text;
                mySetting.FolderTag_Variety = TextBox5.Text;
                mySetting.FolderTag_Movie = TextBox6.Text;
                mySetting.FolderTag_Anime = TextBox7.Text;
                mySetting.FolderTag_Info = TextBox8.Text;
                mySetting.FolderTag_Docum = TextBox9.Text;
                mySetting.FolderTag_Live = TextBox10.Text;
                mySetting.FolderTag_Edu = TextBox11.Text;
                mySetting.FolderTag_Other = TextBox12.Text;
            mySetting.Save();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            mySetting = SettingObj.Read();
            SettingToForm();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure Save all settings?", "Are you sure to exit?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                FormToSettings();
            else
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit Without saving all settings?", "Are you sure to exit?", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                {
                    var withBlock = dialog;
                    withBlock.Filter = "Folders|*.";
                    withBlock.AddExtension = false;
                    withBlock.CheckFileExists = false;
                    withBlock.RestoreDirectory = true;
                    withBlock.Title = "Select record root Folder.";
                    withBlock.FileName = "Select Folder";
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string mPath = dialog.FileName;
                    tbFolder.Text = mPath.EndsWith("Select Folder")?mPath.Remove(mPath.Length-13,13):mPath;
                }
            }
        }
    }
}
