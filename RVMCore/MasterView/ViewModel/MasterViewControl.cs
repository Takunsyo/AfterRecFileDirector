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
                    //TaskBarIcon.Icon = Properties.Resources.NotifyTrayWorking;
                }
                else
                {
                    //TaskBarIcon.Icon = Properties.Resources.NotifyTrayNormal;
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
            //TaskBarIcon.Icon = Properties.Resources.NotifyTrayNormal;
            InitializeMirakurun();
            InitializeEPGStation();
            this.Uploader = new UploaderViewModel();
        }

        public ICommand OpenUploader
        {
            get=>new CustomCommand((x) => {
                if (!(uploaderForm?.IsLoaded ?? false))
                {
                    uploaderForm = new Uploader(this.Uploader, false);
                    uploaderForm.Show();
                }
                else 
                    uploaderForm.Focus();
            });
        }

        public ICommand OpenLogs => new CustomCommand((x) =>
        {
            if (!(mirakurunLog?.IsLoaded ?? false))
            {
                mirakurunLog = new MirakurunLogViewModel(this.mMirakurun);
                mirakurunLog.Show();
            }
            else
                mirakurunLog.Focus();
        });

        public ICommand OpenTV => new CustomCommand((x) =>
        {
            if (!(tvViewer?.IsLoaded ?? false))
            {
                tvViewer = new MirakurunWarpper.MirakurunViewer(this.mMirakurun);
                tvViewer.Show();
            }
            else
                tvViewer.Focus();
        });

        public ICommand OpenSetting => new CustomCommand((x) => {
            Setting mSetForm = new Setting();
            mSetForm.ShowDialog();
        });

        private CloudViewer mCloud;
        public ICommand OpenCloud => new CustomCommand((x) => {
            if (!(mCloud?.IsLoaded ?? false))
            {
                mCloud = new CloudViewer(this.Uploader.Service);
                mCloud.Show();
            }
            else
                mCloud.Focus();
        });

        private RecordedListView mValut;
        public ICommand OpenValut => new CustomCommand((x) => {
            if (!(mValut?.IsLoaded ?? false))
            {
                mValut = new RecordedListView();
                mValut.Show();
            }
            else
                mValut.Focus();
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
                void InitService()
                {
                    try
                    {
                        mMirakurun = new MirakurunWarpper.MirakurunService(setting);
                    }
                    catch(Exception e)
                    {
                        if (MessageBox.Show("Mirakurun server address setting is not correct. " +
                            $"\n {e.Message} \n"+
                            "\nPlease make sure Mirakurun is accessible directly from this operating PC." +
                            "\n\nDo you want to open setting dialog now?", "Setting Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            var dialog = new Setting();
                            if(dialog.ShowDialog()?? false)
                            {
                                setting = SettingObj.Read();
                                InitService();
                            }
                        }
                    }
                }
                InitService();
                if (mMirakurun is null) return;
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
        /// <summary>
        /// raw reserve datas
        /// </summary>
        private List<EPGStationWarpper.Api.Reserve> epgReservRawList { get; set; }
        /// <summary>
        /// all reserves except those are on recording.
        /// </summary>
        public ObservableCollection<EPGView> EpgReserveList 
        {
            get
            {
                if (epgReservRawList is null) return new ObservableCollection<EPGView>();
                var tnow = MirakurunWarpper.MirakurunService.GetUNIXTimeStamp();
                return new ObservableCollection<EPGView>(epgReservRawList?.Where(x =>
                         x.program.startAt > tnow
                    )?.Select(x => new EPGView(x)));
            }
        }
            
        /// <summary>
        /// Main reserve update timer, for server com control.
        /// </summary>
        private System.Timers.Timer EPGUpdateTimer;
        /// <summary>
        /// all reserves under recording.
        /// </summary>
        public ObservableCollection<EPGView> EPGReserves { get; set; }
                

        public void InitializeEPGStation()
        {
            //init EPG
            void InitService()
            {
                try
                {
                    mEPGAccess = new EPGStationWarpper.EPGAccess(setting);
                }
                catch(Exception e)
                {
                    if(MessageBox.Show("EPGStation server address setting is not correct. " +
                        $"\n {e.Message} \n" +
                        "\nPlease make sure EPGStation is accessible directly from this operating PC." +
                        "\n\nDo you want to open setting dialog now?","Setting Error",MessageBoxButton.YesNo,MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        var dialog = new Setting();
                        if (dialog.ShowDialog() ?? false)
                        {
                            setting = SettingObj.Read();
                            InitService();
                        }
                    }
                }
            }

            InitService();
            if (mEPGAccess is null) return;
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
            Getschedule();
            NotifyPropertyChanged(nameof(EpgReserveList));
            //for list view
            object locker = new object();
            if (EPGReserves is null) EPGReserves = new ObservableCollection<EPGView>();
            BindingOperations.EnableCollectionSynchronization(EPGReserves, locker);
            this.Execute(() => { 
                var tmp1st = new ObservableCollection<EPGView>(
                        epgReservRawList?.Where(x =>
                            x.program.startAt <= MirakurunWarpper.MirakurunService.GetUNIXTimeStamp()
                        )?.Select(x => new EPGView(x, true)));
                if (tmp1st.Count <= 0)
                {
                    EPGReserves.Clear();
                    return;
                }
                var tmp2nd = EPGReserves.Intersect(tmp1st);
                if(EPGReserves.Count > 0) { 
                    var tmp3rd = EPGReserves.Except(tmp1st).ToList();

                    foreach (var i in tmp3rd)
                    {
                        EPGReserves.Remove(i);
                    }
                }
                var tmp4th = tmp1st.Except(tmp2nd);
                foreach (var i in tmp4th)
                {
                    EPGReserves.Add(i);
                }
            }
            );
        }

        private void Getschedule()
        {
            var locker = new object();
            if (epgReservRawList is null) epgReservRawList = new List<EPGStationWarpper.Api.Reserve>();
            BindingOperations.EnableCollectionSynchronization(this.epgReservRawList, locker);
            this.Execute(() => {
                epgReservRawList.RemoveAll(x => x.program.endAt <= MirakurunWarpper.MirakurunService.GetUNIXTimeStamp());
                var tmp = mEPGAccess.GetReserves(0, 48);
                if (tmp is null) return;
                foreach(var i in tmp)
                    if(epgReservRawList.Find(x => x.program.id == i.program.id) ==null)
                        epgReservRawList.Add(i);
                epgReservRawList.Sort((x, y) => x.program.startAt.CompareTo(y.program.startAt));
            });
        }

        public class EPGView:ViewModelBase
        {
            public static List<EPGStationWarpper.Api.EPGchannel> EPGchannels;
            public EPGStationWarpper.Api.Reserve body { get; }
            /// <summary>
            /// Secondary timer for reserve inner data update.
            /// </summary>
            private System.Timers.Timer EPGInnerUpdateTimer;
            public EPGView(EPGStationWarpper.Api.Reserve reserve,bool start_timer = false)
            {
                this.body = reserve;
                this.startAt = body.program.startAt;
                this.endAt = body.program.endAt;

                if (start_timer)
                {
                    EPGInnerUpdateTimer = new System.Timers.Timer
                    {
                        Interval = 100,
                        AutoReset = true
                    };
                    EPGInnerUpdateTimer.Elapsed += Timer_Elapsed;
                    EPGInnerUpdateTimer.Start();
                }
            }

            private long startAt;
            private long endAt;

            public void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                var tnow = MirakurunWarpper.MirakurunService.GetUNIXTimeStamp();
                if (tnow < body.program.startAt) return;
                if (tnow > endAt) return;
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
            /// <summary>
            /// For progress bar showing.
            /// </summary>
            public int Now { get; private set; }
            /// <summary>
            /// Total time left.
            /// </summary>
            public string TimeLeft { get; private set; } = "";
            /// <summary>
            /// For show in menu.
            /// </summary>
            public string MenuHeader { get => $"{TimeString(body.program.startAt)}:{Name}"; }
            private string TimeString(long time)
            {
                var T = MirakurunWarpper.MirakurunService.GetDateTime(time).ToLocalTime();
                if(T.Date == DateTime.Now.Date)
                {
                    return $"[本　日 {T.ToString("HH:mm")}]";
                }
                else
                {
                    var culture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");
                    var day = culture.DateTimeFormat.GetDayName(T.DayOfWeek);
                    return $"[{day} {T.ToString("HH:mm")}]";
                }
            }
            public string TimeSpan
            {
                get
                {
                    var TSpanInMin = (int)((body.program.endAt - body.program.startAt) / 60000);
                    switch (TSpanInMin)
                    {
                        case int sec when sec < 60:
                            return $"[{sec.ToString("D2")}分]";
                        default:
                            var h = TSpanInMin / 60;
                            var s = TSpanInMin - (h * 60);
                            return $"[{h.ToString("D2")}時間" + (s == 0 ? "" : $"{s.ToString("D2")}分") + "]";
                    }
                }
            }
            /// <summary>
            /// For show in Popup.
            /// </summary>
            public string Name => $"[{EPGchannels.First(x => x.id == body.program.channelId).name}] {body.program.name}";

               
            public string Genre
            {
                get
                {
                    return body.program.GenreString;
                }
            }
            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if(obj is EPGView)
                {
                    return ((EPGView)obj).body.program.id == this.body.program.id;
                }
                else
                {
                    return false;
                }
            }
            public static bool operator == (EPGView obj1, EPGView obj2)
            {
                return obj1?.Equals(obj2) ?? false;
            }
            public static bool operator !=(EPGView obj1, EPGView obj2)
            {
                return !(obj1?.Equals(obj2) ?? false);
            }
            public override int GetHashCode()
            {
                return this.body.program.id.GetHashCode();
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
