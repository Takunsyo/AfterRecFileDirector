using System.Windows;

namespace RVMCore.MasterView
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class CloudViewer : Window
    {
        public CloudViewerViewModel mView;
            
        public CloudViewer()
        {
            InitializeComponent();
            mView = new CloudViewerViewModel();
            this.DataContext = mView;
        }

        public CloudViewer(GoogleWarpper.GoogleDrive drive)
        {
            InitializeComponent();
            mView = new CloudViewerViewModel(drive);
            this.DataContext = mView;
        }
    }
}
