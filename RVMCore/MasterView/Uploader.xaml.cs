using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace RVMCore.MasterView
{

    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Uploader : Window
    {
        private UploaderViewModel mView;
        private bool Destory;
        public Uploader(UploaderViewModel view ,bool destoryViewWhenClose = true)
        {
            InitializeComponent();
            this.mView = view;
            this.DataContext = mView;
            this.Destory = destoryViewWhenClose;
        }

        public Uploader()
        {
            InitializeComponent();
            mView = new UploaderViewModel();
            this.DataContext = mView;
            mView.NowProcressingContent = "Progress";
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBoxInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(IsTextAllowed(e.Text));
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        protected virtual void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.Destory) return;
            if (mView.IsWorking) { 
                if (MessageBoxResult.Yes != MessageBox.Show("Program is still uploading data. \n Are you SURE to EXIT?", "Yes", MessageBoxButton.YesNo, MessageBoxImage.Information))
                {
                    e.Cancel = true;
                    return;
                }
            }
            mView.Dispose();
        }
    }
}
