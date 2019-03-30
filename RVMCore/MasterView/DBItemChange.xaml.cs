using System.Windows;

namespace RVMCore.MasterView
{
    /// <summary>
    /// DBItemChange.xaml 的交互逻辑
    /// </summary>
    public partial class DBItemChange : Window
    {
        public DBItemChange()
        {
            InitializeComponent();
        }

        public static bool? ShowDialog(string id, string name, string path, long time, bool uploaded, bool visable, Database db)
        {
            var ViewModel = new ViewModel.DBItemChangeViewModel(id, name, path, time, uploaded, visable, db);

            var View = new DBItemChange();
            View.DataContext = ViewModel;
            return View.ShowDialog();
        }
    }
}
