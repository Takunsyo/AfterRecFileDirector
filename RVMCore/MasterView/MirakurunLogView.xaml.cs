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
    /// MirakurunLogView.xaml 的交互逻辑
    /// </summary>
    public partial class MirakurunLogView : Window
    {
        private MirakurunLogViewModel mView;
        public MirakurunLogView(in MirakurunWarpper.MirakurunService service)
        {
            InitializeComponent();
            mView = new MirakurunLogViewModel(service);
            this.DataContext = mView;
        }
        public MirakurunLogView(MirakurunLogViewModel viewModel)
        {
            InitializeComponent();
            mView = viewModel;
            this.DataContext = mView;
        }
        public MirakurunLogView()
        {
            InitializeComponent();
            mView = new MirakurunLogViewModel(
                        new MirakurunWarpper.MirakurunService(
                            SettingObj.Read()));
            this.DataContext = mView;
        }
    }
}
