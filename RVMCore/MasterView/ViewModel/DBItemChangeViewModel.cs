using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace RVMCore.MasterView.ViewModel
{
    internal class DBItemChangeViewModel : ViewModelBase
    {

        public string ID { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Time { get; set; }

        public bool Visable { get; set; }

        public bool IsUploaded { get; set; }

        public ObservableCollection<FileItemView> NewFiles { get; set; }
        
        public ObservableCollection<FileItemView> FolderItems { get; set; }

        public int RSelectedItem { get; set; }

        public int LSelectedItem { get; set; }

        private Database mDB;
        public string fPath
        {
            get;
            set;
        }

        private string root;

        public DBItemChangeViewModel(string id, string name,string path,long time, bool uploaded, bool visable,Database db)
        {
            this.ID = id;
            this.Name = name;
            this.Path = path;
            this.Time = time;
            this.IsUploaded = uploaded;
            this.Visable = visable;
            this.root = SettingObj.Read().StorageFolder;
            this.NewFiles = new ObservableCollection<FileItemView>();
            var t = new FileInfo(System.IO.Path.Combine(root, this.Path)).DirectoryName;
            if (Directory.Exists(t)) this.fPath = t;
            else fPath = string.Empty;
            string tmp = path;
            try
            {
                tmp = System.IO.Path.Combine(root, tmp);
            }
            finally
            {
                this.NewFiles.Add(tmp);
            }
            InitFolder();

            this.mDB = db;
        }

        private void InitFolder() {
            if (!fPath.IsNullOrEmptyOrWhiltSpace())
            {
                var dir = new DirectoryInfo(fPath);
                this.FolderItems = new ObservableCollection<FileItemView>(dir.GetFiles().Select(x => (FileItemView)x.FullName));
                foreach(var i in dir.GetDirectories().Select(x => x.FullName))
                {
                    this.FolderItems.Add(i);
                }

            }
        }

        public ICommand OpenFileCommand => new CustomCommand(OpenFile);
        private void OpenFile(object sender)
        {
            using (var mdlg = new OpenFileDialog())
            {
                mdlg.CheckFileExists = true;
                mdlg.Filter = "Transport Stream(*.ts;*.m2ts)|*.ts;*.m2ts|Meta Data(*.meta;*.xml)|*.meta;*.xml|All file(*.*)|*.*";
                mdlg.Multiselect = true;
                if (mdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string i in mdlg.FileNames)
                    {
                        if (!NewFiles.Any(x => x == i)) NewFiles.Add(i);
                    }
                }
                else return;
            }
        }

        public ICommand ExtendItemCommand => new CustomCommand(ExtendItem);

        private void ExtendItem(object sender)
        {
            if (LSelectedItem < 0) return;
            var mItem = FolderItems[LSelectedItem];
            if (!(sender as FileItemView).FilePath.IsNullOrEmptyOrWhiltSpace())
            {
                FileAttributes attr = File.GetAttributes(mItem);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    fPath = mItem;
                InitFolder();
            }
        }

        public ICommand AddToListCommand => new CustomCommand(AddToList);
        private void AddToList(object sender)
        {
            if (LSelectedItem < 0) return;
            var mItem = FolderItems[LSelectedItem];
            if(!mItem.FilePath.IsNullOrEmptyOrWhiltSpace())
            {
                FileAttributes attr = File.GetAttributes(mItem);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return;
                if(!NewFiles.Any(x => x == mItem)) NewFiles.Add(mItem);
            }
        }
        public ICommand RemoveFromListCommand => new CustomCommand(RemoveFromList);

        private void RemoveFromList(object sender)
        {
            if (RSelectedItem < 0) return;
            NewFiles.RemoveAt(RSelectedItem);
        }
        public ICommand GoUpDirCommand => new CustomCommand(GoUpDir);

        private void GoUpDir(object x)
        {
            if (fPath.IsNullOrEmptyOrWhiltSpace()) return;
            var info = new DirectoryInfo(fPath);
            fPath = info.Parent?.FullName ?? string.Empty;
            InitFolder();
        }
        public ICommand OKCommand => new CustomCommand(OK);

        private void OK(object x)
        {
            var win = x as Window;

            if(System.Windows.MessageBox.Show($"Are you sure to apply change to [{ID}] ?",
                "Question",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if(!NewFiles.Any( y => y[root]==this.Path))mDB.DeleteRecord(this.ID);
                var tID = this.ID;
                foreach(var i in NewFiles)
                {
                    var t = new FileInfo(i).Name;
                    if(i[root] == this.Path)                    
                        mDB.UpDateFile(tID, i[root], t, this.Time);                    
                    else                    
                        mDB.AddDataItem(i[root], t, this.Time, tID, null);
                    
                    mDB.SetUploadStatus(tID, this.Visable, this.IsUploaded);
                    tID = Database.GenerateID();
                }
                var s = "";
                System.Windows.MessageBox.Show("OK!");
                win.DialogResult = true;
            }
        }

        public ICommand CancelCommand => new CustomCommand(Cancel);

        private void Cancel(object x)
        {
            var win = x as Window;
            win.DialogResult = false;
        }

        public class FileItemView : ViewModelBase
        {
            public string FilePath { get; set; }

            public string Name { get { return System.IO.Path.GetFileName(FilePath); } }

            public string this[string root] => this.FilePath.Remove(0, root.Length);

            public FileItemView(string path)
            {
                this.FilePath = path;
            }

            public static bool operator == (FileItemView input, string output)
            {
                return input.FilePath == output;
            }

            public static bool operator !=(FileItemView input, string output)
            {
                return input.FilePath != output;
            }

            public override string ToString()
            {
                return this.FilePath;
            }

            public override bool Equals(object obj)
            {
                return this.FilePath.Equals(obj);
            }

            public override int GetHashCode()
            {
                return this.FilePath.GetHashCode();
            }

            public static implicit operator string(FileItemView input)
            {
                return input.FilePath;
            }

            public static implicit operator FileItemView(string input)
            {
                return new FileItemView(input);
            }
        }
    }
}
