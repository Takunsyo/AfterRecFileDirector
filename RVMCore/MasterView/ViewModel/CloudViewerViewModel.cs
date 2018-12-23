using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RVMCore.GoogleWarpper;
using Google.Apis.Drive.v3.Data;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.Generic;

namespace RVMCore.MasterView
{
    public class CloudViewerViewModel : ViewModelBase, IDisposable
    {
        private GoogleDrive gService;

        public ObservableCollection<DriveTree> TreeView { get; set; }

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


}
