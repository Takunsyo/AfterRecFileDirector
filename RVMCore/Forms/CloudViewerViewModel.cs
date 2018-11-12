using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RVMCore.Forms.IconHelper.IconReader;
using RVMCore.GoogleWarpper;
using Google.Apis.Drive.v3.Data;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.Generic;

namespace RVMCore.Forms
{
    public class CloudViewerViewModel : ViewModelBase, IDisposable
    {
        private GoogleDrive gService;

        private ObservableCollection<DriveTree> _treeView;
        public ObservableCollection<DriveTree> TreeView
        {
            get { return _treeView; }
            set { SetProperty(ref _treeView, value); }
        }

        public CloudViewerViewModel()
        {
            gService = new GoogleDrive();
            TreeView =new ObservableCollection<DriveTree>();
            TreeView.Add(new DriveTree(gService.Root, gService));
            var q = "sharedWithMe = true and mimeType = 'application/vnd.google-apps.folder'";
            var mfile = gService.GetGoogleFiles(q, false).Where(x => x.Parents == null);
            foreach (var i in mfile)
            {
                TreeView.Add(new DriveTree(i, gService));
            }
        }

        public CloudViewerViewModel(GoogleDrive service)
        {
            gService = service;
            TreeView = new ObservableCollection<DriveTree>();
            TreeView.Add(new DriveTree(gService.Root, gService));
            var q = "sharedWithMe = true and mimeType = 'application/vnd.google-apps.folder'";
            var mfile = gService.GetGoogleFiles(q, false).Where(x => x.Parents == null);
            foreach (var i in mfile)
            {
                TreeView.Add(new DriveTree(i, gService));
            }
        }

        public void Dispose()
        {
            gService.Dispose();
        }
    }

    public class DriveTree : ViewModelBase
    {
        private bool isGoogle = true;
        /// <summary>
        /// Base service of this object.
        /// </summary>
        dynamic Service;

        #region"Properties"
        private bool _isExpanded = false;
        private bool _isSelected = false;
        private DriveTree _parent = null;
        private ObservableCollection<DriveTree> _childs = null;
        private string _text;
        dynamic _odds = null;
        private static ObservableCollection<DriveTree> Dummy = new ObservableCollection<DriveTree>() { new DriveTree() };

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        /// <summary>
        /// The item has been expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set {
                SetProperty(ref _isExpanded, value);
                if ((value & this.Expandible &  (this._childs != null && this._childs.Equals(Dummy)))) ExpandMe();
            }
        }

        /// <summary>
        /// The parent of current object, null if is root.
        /// </summary>
        public DriveTree Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }

        /// <summary>
        /// Child items of this cbject.
        /// <para>Will call <see cref="ExpandMe"/> method when getting value of this property.</para>
        /// </summary>
        public ObservableCollection<DriveTree> Childs
        {
            get
            {
                return _childs;
            }
            set { SetProperty(ref _childs, value); }
        }

        /// <summary>
        /// View name of this object.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        
        /// <summary>
        /// View Icon of this object.
        /// </summary>
        public ImageSource Icon
        {
            get {
                if (this.Text.IsNullOrEmptyOrWhiltSpace()) return null;
                try
                {
                    return GetFileIcon(System.IO.Path.GetExtension(this.Text),this.Expandible);
                }
                catch
                {
                    var ext = this.Text.Substring(this.Text.LastIndexOf('.'));
                    return GetFileIcon(ext, this.Expandible);
                }
                }
        }

        /// <summary>
        /// The metadata include in this object.
        /// </summary>
        public dynamic Odds
        {
            get { return _odds; }
            set { SetProperty(ref _odds, value); }
        }

        /// <summary>
        /// indecate that this object is a folder.
        /// </summary>
        public bool Expandible
        {
            get {
                if (this.isGoogle)
                {
                    if (Odds is null) return true;
                    return ((File)Odds).IsFolder();
                }
                else
                {
                    throw new NotSupportedException("Currently only support Google Drive Apis.");
                }
            }
        }
        #endregion
        public DriveTree()
        {
            this.Text = "DUMMY__DUMMY";
        }

        public DriveTree(GoogleDrive service)
        {
            this.Childs = new ObservableCollection<DriveTree>();
            this.Service = service; if (Expandible)
            {
                this.Childs = Dummy;
            }
        }

        public DriveTree(File mFile, GoogleDrive service)
        {
            this.Parent = null;
            this.Text = mFile.Name;
            this.Odds = mFile;
            this.isGoogle = true;
            this.Service = service; if (Expandible)
            {
                this.Childs = Dummy;
            }
        }

        public DriveTree(File mFile ,GoogleDrive service, DriveTree mParent = null)
        {
            this.Parent = mParent;
            this.Text = mFile.Name;
            this.Odds = mFile;
            this.isGoogle = true;
            this.Service = service;
            if (Expandible)
            {
                this.Childs = Dummy;
            }
        }
        private async void ExpandMe()
        {
            if (Odds is null) return;
            if (Expandible)
            {
                if (this.isGoogle)
                {
                    this.Childs = new ObservableCollection<DriveTree>();
                    string token = "";
                    do
                    {
                        object _mLock = new object();
                        BindingOperations.EnableCollectionSynchronization(this.Childs, _mLock);
                        var id = this.Odds.Id;
                        var q = string.Format("'{0}' in parents", id);
                        Tuple<IEnumerable< File >,string> tuple = await Service.GetGoogleFilesAsync(token, 20, q, false);
                        foreach (var i in tuple.Item1)
                        {
                            this.Execute(() =>
                            {
                                lock (_mLock)
                                {
                                    this.Childs.Add(new DriveTree(i, Service, this));
                                }
                            });
                        }
                        token = tuple.Item2;
                    } while (token != null);

                }
            }
        }
    }

}
