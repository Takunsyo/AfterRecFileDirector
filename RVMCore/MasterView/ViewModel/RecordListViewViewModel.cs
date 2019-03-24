using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RVMCore.MasterView.ViewModel
{
    internal class RecordListViewViewModel:ViewModelBase ,IDisposable
    {

        public static SettingObj Setting;
        private DataSet _mainTable;
        public DataTable MainTable
        {
            get
            {
                if (_mainTable.Tables.Count > 0)
                    return _mainTable.Tables[0];
                else
                    return null;
            }
        }
        private Database database;

        private bool isRoot = false;

        public bool LoadAll { get; set; } = false;

        public DateTime TimeFrom { get; set; } = DateTime.Now.AddYears(-1);

        public DateTime TimeTo { get; set; } = DateTime.Now;

        public DataRowView SelectedItem { get; set; } = null;

        public bool IsReady { get; set; } = false;

        public RecordListViewViewModel()
        {
            Setting = SettingObj.Read();
            _mainTable = new DataSet();
            if (Setting.DataBase != "mysql") throw new InvalidOperationException("Database type is unsupported!");
            var work = new System.Threading.Thread(
                new System.Threading.ThreadStart(
                    new Action(() => {
                        database = new Database(Setting.DataBase_Addr, Setting.DataBase_User, Setting.DataBase_Pw, Setting.DataBase_Port ?? 3306);
                        IsReady = true;
                        })));
            work.IsBackground = true;
            work.Start();
        }

        private void LoadAction(object x)
        {
            database.LoadData(ref _mainTable,TimeFrom,TimeTo,LoadAll);
            NotifyPropertyChanged(nameof(this.MainTable));
        }

        public void Dispose()
        {
            ((IDisposable)database).Dispose();
            _mainTable?.Dispose();
        }

        public ICommand LoadCommand => new CustomCommand(LoadAction);

        private string GetSelectedPath()
        {
            if (this.SelectedItem is null) return null;
            var data = this.SelectedItem as DataRowView;
            if (data is null) return null;
            var fPath = data.Row.Field<string>("path");
            if (fPath.IsNullOrEmptyOrWhiltSpace()) return null;
            try
            {
                fPath = System.IO.Path.Combine(Setting.StorageFolder, fPath);
            }
            catch
            {
                return null;
            }
            if (System.IO.File.Exists(fPath))
            {
                return fPath;
            }
            else return null;
        }

        private void OpenFolderAction(object x)
        {
            var p = GetSelectedPath();
            if (!p.IsNullOrEmptyOrWhiltSpace())
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{p}\"");
            }
        }

        public ICommand OpenFolderCommand => new CustomCommand(OpenFolderAction);

        private void OpenMetaAction(object x)
        {
            var p = GetSelectedPath();
            if (!p.IsNullOrEmptyOrWhiltSpace()) { 
                p = p.Substring(0, p.LastIndexOf('.')) + ".meta";
                if (System.IO.File.Exists(p))
                {
                    System.Diagnostics.Process.Start(p);
                }
            }
        }

        public ICommand OpenMetaCommand => new CustomCommand(OpenMetaAction);

        private void GetRootPrivilege()
        {
            var wpfwindow = new RVMCore.MasterView.PasswordCheckDialog(this.database);
            var result = wpfwindow.ShowDialog();
            this.isRoot = (result ?? false);
        }

        private void RemoveLine(object x)
        {
            if (this.SelectedItem is null) return;
            var data = this.SelectedItem as DataRowView;
            if (data is null) return;
            var id = data.Row.Field<string>("id");
            var name = data.Row.Field<string>("name");
            if (id.IsNullOrEmptyOrWhiltSpace()) return;
            if(MessageBox.Show("Are you sure to remove this record?","Attention",
                MessageBoxButton.YesNo,MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                if (!this.isRoot) GetRootPrivilege();
                if (!this.isRoot) return;
                if (this.database.DeleteRecord(id))
                {
                    "DB record DELETE for file [{0}] ID='{1}'".InfoLognConsole(name, id);
                    MessageBox.Show("Success!");
                    this.LoadAction(null);
                }
                else
                {
                    MessageBox.Show("Failed! See log for more infomation.");
                }
            }
        }

        public ICommand RemoveLineCommand => new CustomCommand(RemoveLine);
    }

    public class BoolToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }

    public class LongToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MirakurunWarpper.MirakurunService.GetDateTime(long.Parse(value.ToString())).ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MirakurunWarpper.MirakurunService.GetUNIXTimeStamp(DateTime.ParseExact((string)value, "yyyy-MM-ddTHH:mm:ss", null));
        }
    }

    public class DataToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            var data = value as DataRowView;
            if (data is null) return false;
            var fPath = data.Row.Field<string>("path");
            if (fPath.IsNullOrEmptyOrWhiltSpace()) return false;
            try
            {
                fPath = System.IO.Path.Combine(RecordListViewViewModel.Setting.StorageFolder, fPath);
            }
            catch
            {
                return false;
            }
            if (System.IO.File.Exists(fPath))
            {
                return true;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
