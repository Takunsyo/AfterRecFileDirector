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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string[] margs = textBox1.Text.Split(' ');
            //RVMCore.TVAFT.SortFile(margs);
            MessageBox.Show(RVMCore.Share.FindTitle(textBox1.Text));
            //MessageBox.Show(RVMCore.Share.GetTimeSpan(DateTime.Now, DateTime.Now.AddMonths(-1)));
        }
    }
}
