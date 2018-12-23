using Google.Apis.Drive.v3.Data;
using RVMCore.GoogleWarpper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using static RVMCore.Forms.IconHelper.IconReader;

namespace RVMCore.MasterView
{

    public class DriveTree : ViewModelBase
    {
        private bool isGoogle = true;
        /// <summary>
        /// Base service of this object.
        /// </summary>
        dynamic Service;

        #region"Properties"
        private bool _isExpanded = false;
        private static ObservableCollection<DriveTree> Dummy = new ObservableCollection<DriveTree>() { new DriveTree() };

        public bool IsSelected { get; set; }

        /// <summary>
        /// The item has been expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if ((value & this.Expandible & (this.Childs != null && this.Childs.Equals(Dummy)))) ExpandMe();
                this._isExpanded = value;
            }
        }

        /// <summary>
        /// The parent of current object, null if is root.
        /// </summary>
        public DriveTree Parent { get; set; }

        /// <summary>
        /// Child items of this cbject.
        /// <para>Will call <see cref="ExpandMe"/> method when getting value of this property.</para>
        /// </summary>
        public ObservableCollection<DriveTree> Childs { get; set; }

        /// <summary>
        /// View name of this object.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// View Icon of this object.
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                if (this.Text.IsNullOrEmptyOrWhiltSpace()) return null;
                try
                {
                    return GetFileIcon(System.IO.Path.GetExtension(this.Text), this.Expandible);
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
        public dynamic Odds { get; set; }

        /// <summary>
        /// indecate that this object is a folder.
        /// </summary>
        public bool Expandible
        {
            get
            {
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

        #region Constrasters
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

        public DriveTree(File mFile, GoogleDrive service, DriveTree mParent = null)
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
        #endregion
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
                        Tuple<IEnumerable<File>, string> tuple = await Service.GetGoogleFilesAsync(token, 20, q, false);
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
