﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RVMCore.MasterView
{
    /// <summary>
    /// PasswordCheckDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordCheckDialog : Window,INotifyPropertyChanged
    {
        public PasswordCheckDialog(Database database)
        {
            InitializeComponent();
            mBaseService = database;
            this.DataContext = this;
            BitmapImage bitmapImage = new BitmapImage(new Uri("/My%20Application;component/passwd.png", UriKind.Relative));
            //this.XImage.Source = bitmapImage;
        }

        private Database mBaseService;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (!PassBox.Password.IsNullOrEmptyOrWhiltSpace() && mBaseService.SuperLogonCheck(PassBox.Password))
            {
                this.DialogResult = true;
            }
            else
            {
                PassBox.Password = "";
                MessageBox.Show("Password not correct!!");
            }
        }
        
        public string PassWD { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
