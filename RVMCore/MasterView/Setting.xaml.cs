using System.Windows;

namespace RVMCore.MasterView
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            this.DataContext = new SettingViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you ready to cancel?", "Cancel?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
