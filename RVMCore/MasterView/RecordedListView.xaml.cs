using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RVMCore.MasterView
{
    /// <summary>
    /// RecordedListView.xaml 的交互逻辑
    /// </summary>
    public partial class RecordedListView : Window
    {

        private ViewModel.RecordListViewViewModel myModel = new ViewModel.RecordListViewViewModel();
        public RecordedListView()
        {
            InitializeComponent();
            this.DataContext = myModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myModel.Dispose();
        }
    }
}
