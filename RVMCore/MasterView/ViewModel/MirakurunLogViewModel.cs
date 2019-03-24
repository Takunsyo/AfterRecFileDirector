using RVMCore.MirakurunWarpper;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace RVMCore.MasterView
{
    public class MirakurunLogViewModel :ViewModelBase
    {
        public MirakurunService Service { get; private set; }
        public MirakurunLogViewModel(in MirakurunService mirakurun)
        {
            this.Service = mirakurun;
            this.Service.LogRecived += Service_LogRecived;
            this.Service.SubscribeLogs();
            this.Logs = new ObservableCollection<LogView>();
            this.mView = new MirakurunLogView(this);
        }

        private MirakurunLogView mView;
        public ObservableCollection<LogView> Logs { get; set; }

        public bool IsLoaded
        {
            get
            {
                if(!(mView?.IsLoaded ?? false))
                {
                    mView = new MirakurunLogView(this);
                }
                return true;
            }
        }

        public void Show() => mView?.Show();

        public void Focus() => mView?.Focus();

        private void LogsFailedCallback(object sender)
        {
            var serv = (MirakurunService)sender;
            Thread.Sleep(3000);
            //serv.SubscribeLogs(LogsFailedCallback);
        }

        private void Service_LogRecived(object sender, string log)
        {
            var locker = new object();
            BindingOperations.EnableCollectionSynchronization(this.Logs, locker);
            this.Execute(() =>
            {
                this.Logs.Add(new LogView(log));
                if (Logs.Count >= 1000) Logs.RemoveAt(0);
            });
        }

        public class LogView
        {
            public LogView(string logString)
            {
                LogString = logString;
                Type = LogType.Other;
                var firstPart =logString.Substring(0, logString.IndexOf(' '));
                if (!DateTime.TryParse(firstPart, out var logTime))
                    return;
                else
                {
                    var typePart = logString.Replace(firstPart,"").Trim();
                    typePart = typePart.Substring(0, typePart.IndexOf(':'));
                    if (!typePart.IsNullOrEmptyOrWhiltSpace())
                    {
                        switch (typePart)
                        {
                            case "info": this.Type = LogType.Info;
                                break;
                            case "warn": this.Type = LogType.Warn;
                                break;
                            case "error":this.Type = LogType.Error;
                                break;
                            default:this.Type = LogType.Other;
                                break;
                        }
                    }
                }
            }

            public string LogString { get; }

            public LogType Type { get; }

            public enum LogType
            {
                Info , Warn, Error ,Other
            };
        }
    }

    [ValueConversion(typeof(MirakurunLogViewModel.LogView.LogType), typeof(Color))]
    public class LogTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is MirakurunLogViewModel.LogView.LogType))
                throw new ArgumentException("value not of type StateValue");
            MirakurunLogViewModel.LogView.LogType sv = (MirakurunLogViewModel.LogView.LogType)value;
            //sanity checks
            switch (sv)
            {
                case MirakurunLogViewModel.LogView.LogType.Error:
                    return new SolidColorBrush(Colors.Red);
                case MirakurunLogViewModel.LogView.LogType.Info:
                    return new SolidColorBrush(Colors.White);
                case MirakurunLogViewModel.LogView.LogType.Warn:
                    return new SolidColorBrush(Colors.Orange);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
