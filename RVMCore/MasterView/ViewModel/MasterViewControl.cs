using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace RVMCore.MasterView
{
    public partial class MasterViewControl: Window , IDisposable ,INotifyPropertyChanged
    {
        private MirakurunWarpper.MirakurunService mMirakurun;
        private EPGStationWarpper.EPGAccess mEPGAccess;
        private SettingObj setting;
        //private System.Windows.Controls.ContextMenu mMenu;
        //Halt ro prevent app to exit.
        //private Thread Halt = new Thread(new ThreadStart(() => { Thread.Sleep(Timeout.Infinite); }));

        //Premade menu items for item container.

        #region properties

        private bool _IsWorking = false;
        private bool IsWorking
        {
            get => _IsWorking;
            set
            {
                _IsWorking = value;
                if (value)
                {
                    TaskBarIcon.Icon = Properties.Resources.NotifyTrayWorking;
                }
                else
                {
                    TaskBarIcon.Icon = Properties.Resources.NotifyTrayNormal;
                }
            }
        }
        #endregion

        public void Dispose()
        {
            //((IDisposable)mMenu).Dispose();
            //((IDisposable)mTaskBarIcon).Dispose();
            mMirakurun.Dispose();
            //((IDisposable)mEPGAccess).Dispose();
            Uploader.Dispose();
            //Halt.Abort();
        }

        public UploaderViewModel Uploader { get; set; }
        private Uploader uploaderForm;
        private MirakurunLogViewModel mirakurunLog;
        private MirakurunWarpper.MirakurunViewer tvViewer;

        public MasterViewControl()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            InitializeComponent();
            setting = SettingObj.Read();
            this.DataContext = this;
            this.TaskBarIcon.DataContext = this;
            TaskBarIcon.Icon = Properties.Resources.NotifyTrayNormal;
            InitializeMirakurun();
            InitializeEPGStation();
            this.Uploader = new UploaderViewModel();
        }

        public ICommand OpenUploader
        {
            get=>new CustomCommand(() => {
                if (!(uploaderForm?.IsLoaded ?? false))
                    uploaderForm = new Uploader(this.Uploader, false);
                uploaderForm.Show();
            });
        }

        public ICommand OpenLogs => new CustomCommand(() =>
        {
            if (!(mirakurunLog?.IsLoaded ?? false))
                mirakurunLog = new MirakurunLogViewModel(this.mMirakurun);
            mirakurunLog.Show();
        });

        public ICommand OpenTV => new CustomCommand(() =>
        {
            if (!(tvViewer?.IsLoaded ?? false))
                tvViewer = new MirakurunWarpper.MirakurunViewer(this.mMirakurun);
            tvViewer.Show();
        });

        public ICommand OpenSetting => new CustomCommand(() => {
            if(!IsWindowOpen<Setting>())(new Setting()).Show();
        });

        public ICommand OpenCloud => new CustomCommand(() => {
            if (!IsWindowOpen<CloudViewer>()) (new CloudViewer(this.Uploader.Service)).Show();
        });

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Thread work = new Thread(new ThreadStart(() => { 
                if(MessageBox.Show(
                    "Do you really want to exit AfterRecordDirector? \nThis will disenable watching and upload.",
                    "Are you sure?",MessageBoxButton.YesNo,MessageBoxImage.Warning, MessageBoxResult.No,MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.Yes)
                {
                    Dispatcher.Invoke(() => { 
                        this.Close();
                        this.Dispose();
                    });
                }
            }));
            work.Start();
        }

        #region Mirakurun

        public ObservableCollection<MirakurunWarpper.Apis.Tuner> Tuners { get; set; }

        public void InitializeMirakurun()
        {
            var work = new Thread(new ThreadStart(() => { 
                
                //init mirakurun
                mMirakurun = new MirakurunWarpper.MirakurunService(setting);
                InitTunerView();
                mMirakurun.EventRecived += MMirakurun_EventRecived;                
                mMirakurun.SubscribeEvents(null,MirakurunWarpper.Apis.ResourceType.tuner);
            }));
            void InitTunerView()
            {
                var tmp = mMirakurun.GetTuners();
                var locker = new object();
                this.Tuners = new ObservableCollection<MirakurunWarpper.Apis.Tuner>();
                BindingOperations.EnableCollectionSynchronization(this.Tuners, locker);
                this.Execute(() => {
                    this.Tuners = new ObservableCollection<MirakurunWarpper.Apis.Tuner>();
                    foreach (var i in tmp)
                    {
                        Tuners.Add(i);
                    }
                });
            }
            work.Start();
        }
        //Call back methods
        private void EventFailedCallback(object sender)
        {
            var serv = (MirakurunWarpper.MirakurunService)sender;
            Thread.Sleep(3000);
            //serv.SubscribeEvents(EventFailedCallback);
        }
        //Recived Events.
        private void MMirakurun_EventRecived(object sender, MirakurunWarpper.Apis.Event events)
        {
            switch (events.resource)
            {
                case MirakurunWarpper.Apis.ResourceType.tuner:
                    var evtTuner = (MirakurunWarpper.Apis.Tuner)events.Data;
                    var index = Tuners.IndexOf(Tuners.First(x => x.name == evtTuner.name));
                    var locker = new object();
                    BindingOperations.EnableCollectionSynchronization(this.Tuners, locker);
                    this.Execute(() => { Tuners[index] = evtTuner; });
                    break;
                case MirakurunWarpper.Apis.ResourceType.service:

                    break;
                case MirakurunWarpper.Apis.ResourceType.program:

                    break;
            }
        }

        #endregion

        #region EPGStation
        private List<EPGStationWarpper.Api.Reserve> epgReserveList;
        private System.Timers.Timer EPGUpdateTimer;
        public ObservableCollection<EPGView> EPGReserves { get; set; }

        public void InitializeEPGStation()
        {
            //init EPG
            mEPGAccess = new EPGStationWarpper.EPGAccess(setting);
            var work = new Thread(new ThreadStart(() => {
                EPGUpdateTimer = new System.Timers.Timer
                {
                    AutoReset = true,
                    Interval = 10 * 1000
                };
                EPGUpdateTimer.Elapsed += EPGUpdate;
                EPGUpdateTimer.Start();
                EPGView.EPGchannels = new List<EPGStationWarpper.Api.EPGchannel>(mEPGAccess.GetChannels());
            }));
            work.Start();
        }

        private void EPGUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            //if (epgReserveList is null) Getschedule();
            //epgReserveList.RemoveAll(x => x.program.endAt <= MirakurunWarpper.MirakurunService.GetUNIXTimeStamp());
            //if (epgReserveList is null && epgReserveList.Count <3) Getschedule();
            Getschedule();
            if (epgReserveList is null) return;
            if (EPGReserves is null)
            {
                EPGReserves = new ObservableCollection<EPGView>();
            }
            //epgReserveList.Sort(delegate (EPGStationWarpper.Api.Reserve x, EPGStationWarpper.Api.Reserve y) 
            //    {
            //        return x.program.startAt.CompareTo(y.program.startAt);
            //    });
            var tmp = epgReserveList.TakeWhile(x => x.program.startAt <= MirakurunWarpper.MirakurunService.GetUNIXTimeStamp());
            var locker = new object();
            BindingOperations.EnableCollectionSynchronization(this.EPGReserves, locker);
            this.Execute(() => {
                if (tmp.Count() <= 0) { EPGReserves.Clear();return; }
                foreach (var i in tmp)
                {
                    if(!EPGReserves.Any(x=> x.body.program.id == i.program.id))
                    EPGReserves.Add(new EPGView(i));
                }
                if (EPGReserves.Count == tmp.Count()) return;
                while (true)
                {
                    try { 
                        var i = EPGReserves.First(x => !tmp.Any(y => x.body.program.id == y.program.id));
                        EPGReserves.Remove(i);
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }

        private void Getschedule()
        {
            var locker = new object();
            if (epgReserveList is null) epgReserveList = new List<EPGStationWarpper.Api.Reserve>();
            BindingOperations.EnableCollectionSynchronization(this.epgReserveList, locker);
            this.Execute(() => {
                var tmp = mEPGAccess.GetReserves(0, 24);
                if (tmp is null) return;
                epgReserveList = new List<EPGStationWarpper.Api.Reserve>(tmp);
            });
        }

        public class EPGView:ViewModelBase
        {
            public static List<EPGStationWarpper.Api.EPGchannel> EPGchannels;
            public EPGStationWarpper.Api.Reserve body { get; }
            private System.Timers.Timer timer;
            public EPGView(EPGStationWarpper.Api.Reserve reserve)
            {
                this.body = reserve;
                this.startAt = body.program.startAt;
                this.endAt = body.program.endAt;
                timer = new System.Timers.Timer
                {
                    Interval = 100,
                    AutoReset = true
                };
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }

            private long startAt;
            private long endAt;

            private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                long tnow = MirakurunWarpper.MirakurunService.GetUNIXTimeStamp();
                if (tnow > endAt) { timer.Stop(); return; }
                Now =(int)( tnow - startAt);
                TimeLeft = (MirakurunWarpper.MirakurunService.GetDateTime(endAt)-DateTime.UtcNow).ToString("hh\\:mm\\:ss");
            }

            public int Max
            {
                get
                {
                    return (int)(endAt - startAt);
                }
            }
            
            public int Now { get; private set; }
            public string TimeLeft { get; private set; }
            public string Name
            {
                get
                {
                    return "[" + EPGchannels.First(x => x.id == body.program.channelId).name + "] " + body.program.name;
                }
            }

            public string Genre
            {
                get
                {
                    return body.program.GenreString;
                }
            }
        }
        #endregion

        public bool IsWindowOpen<T>(string name = "") where T : Window
        {
            
            return string.IsNullOrEmpty(name)
                ? Application.Current?.Windows.OfType<T>()?.Any() ?? false
                : Application.Current?.Windows.OfType<T>()?.Any(w => w.Name.Equals(name)) ?? false;
            
        }

        #region ViewModel
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Gets the dispatcher used by this view model to execute actions on the thread it is associated with.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Threading.Dispatcher"/> used by this view model to 
        /// execute actions on the thread it is associated with. 
        /// The default value is the <see cref="System.Windows.Threading.Dispatcher.CurrentDispatcher"/>.
        /// </value>
        protected Dispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
        }
        private readonly Dispatcher _dispatcher;
        /// <summary>
        /// Executes the specified <paramref name="action"/> synchronously on the thread 
        /// the <see cref="ViewModelBase"/> is associated with.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to execute.</param>
        protected void Execute(Action action)
        {
            if (this.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.DataBind, action);
            }
        }
        #endregion
    }
}
