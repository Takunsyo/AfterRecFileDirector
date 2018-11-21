using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RVMCore.Forms
{
    
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Uploader : Window
    {
        private UploaderViewModel mView = new UploaderViewModel();

        public Uploader()
        {
            InitializeComponent();

            this.DataContext = mView;
            mView.NowProcressingContent = "Progress";
            //mView.ProcessBarTextNow = "Now procress 50%";
            //mView.ProcessGenMax = 100;
            //mView.ProcessGenValue = 1;
            //mView.ProcessNowMax = 100;
            //mView.ProcessNowValue = 50;
            //mView.ProcessBarTextGen = "Genral progress 1%";
        }
        public void Open_Click(object sender, EventArgs e)
        {
            mView.Open_Click(sender, e);
        }

        public void Start_Click(object sender, EventArgs e)
        {
            mView.Start_Click(sender, e);
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
            if (mView.IsWorking) { 
                if (MessageBoxResult.Yes != MessageBox.Show("Program is still uploading data. \n Are you SURE to EXIT?", "Yes", MessageBoxButton.YesNo, MessageBoxImage.Information))
                {
                    e.Cancel = true;
                    return;
                }
            }
            mView.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mView.UpItem(sender, e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mView.RemoveItem(sender, e);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mView.DownItem(sender, e);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mView.ResetThreads();
        }
    }


}
