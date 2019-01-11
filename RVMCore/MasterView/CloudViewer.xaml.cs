using System;
using System.Windows;
using System.Windows.Input;

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

        Point _startPoint;
        bool IsDragging = false;

        /// <summary>
        /// Mouse left button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void MainView_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)

            {

                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||

                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)

                {

                    StartDrag(e);


                }

            }
        }
        private void StartDrag(MouseEventArgs e)

        {

            IsDragging = true;
            object temp = this.MainView.SelectedItem;
            DataObject data = null;

            data = new DataObject("inadt", temp);

            if (data != null)
            {
                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDropEffects de = DragDrop.DoDragDrop(this.MainView, data, dde);
            }
            IsDragging = false;
        }

        private void MainView_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void MainView_Drop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(typeof(FolderViewModel)))
            //{
            //    var folderViewModel = e.Data.GetData(typeof(FolderViewModel)) as FolderViewModel;
            //    var treeViewItem =
            //        FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

            //    var dropTarget = treeViewItem.Header as FolderViewModel;

            //    if (dropTarget == null || folderViewModel == null)
            //        return;

            //    folderViewModel.Parent = dropTarget;
            //}
        }
    }
    
}
