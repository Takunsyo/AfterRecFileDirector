using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Documents;
using RVMCore.Forms;

namespace RVMCore.MirakurunWarpper
{
    class MirakurunViewerView : ViewModelBase
    {

        public MirakurunViewerView()
        {
            //services = new MirakurunService("http://127.0.0.1:40772/");
            try
            {
                services = new MirakurunService(SettingObj.Read());
            }
            catch(System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message+
                    "\nPlease make sure server is up online."+
                    "\nServer_ADDR:[" + SettingObj.Read().Mirakurun_ServiceAddr +"]", 
                    "ERROR",
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
                Environment.Exit(1);
            }
            catch
            {
                Environment.Exit(1);
            }
            _ChannelList = new Dictionary<ChannelType, ObservableCollection<Apis.Service>>();
            _ChannelList.Add(ChannelType.GR, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.GR)));
            _ChannelList.Add(ChannelType.BS, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.BS)));
            _ChannelList.Add(ChannelType.CS, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.CS)));
            _ChannelList.Add(ChannelType.SKY, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.SKY)));
        }
        private MirakurunService services;
        private List<Apis.Program> ProgramsBank;

        private ChannelType _ChannelType = ChannelType.GR;
        public ChannelType ChannelType
        {
            get
            {
                return _ChannelType;
            }
            set
            {
                SetProperty(ref _ChannelType, value);
                NotifyPropertyChanged("ChannelList");
            }
        }

        private Dictionary<ChannelType, ObservableCollection<Apis.Service>> _ChannelList;
        public ObservableCollection<Apis.Service> ChannelList
        {
            get
            {
                return _ChannelList[ChannelType];
            }
        }

        private Apis.Service _Selected;
        public Apis.Service Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                SetProperty(ref _Selected, value);
            }
        }

        public Uri ViewUri { get; private set; }

        private Timer timer;
        public void ChangeChannel()
        {
            this.ViewUri =new Uri(services.GetServiceStreamPath(this.Selected.id));
            NotifyPropertyChanged("ViewUri");
            Apis.Program tmp = null;
            GetPrograms(this.Selected.serviceId);
            try
            {
                tmp = this.ProgramsBank.First((x) => {
                    var tNow = MirakurunService.GetUNIXTimeStamp();
                    var endT = x.startAt + x.duration;
                    return (x.startAt <= tNow) && (endT >= tNow);
                });
            }
            catch
            {
                System.Windows.MessageBox.Show("Server data need to be updated!");
                return;
            }
            this.NowProgram = tmp;
            NotifyPropertyChanged("Description");
            var v = this.NowProgram.startAt + this.NowProgram.duration - MirakurunService.GetUNIXTimeStamp();
            this.timer = new Timer();
            this.timer.AutoReset = true;
            this.timer.Interval = v;
            this.timer.Elapsed += Timer_Elapsed;
            this.timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Apis.Program tmp = null;
            //GetPrograms(this.Selected.serviceId);
            try
            {
                tmp = this.ProgramsBank.First((x) => {
                    var tNow = MirakurunService.GetUNIXTimeStamp();
                    var endT = x.startAt + x.duration;
                    return (x.startAt <= tNow) && (endT >= tNow);
                });
            }
            catch
            {
                GetPrograms(this.Selected.serviceId);
                Timer_Elapsed(sender, e);
                System.Windows.MessageBox.Show("Server data need to be updated!");
            }
            this.NowProgram = tmp;
            NotifyPropertyChanged("Description");
            var v = this.NowProgram.startAt + this.NowProgram.duration - MirakurunService.GetUNIXTimeStamp();
            if (v <= 0) return;
            this.timer.Interval = v;
            this.timer.Elapsed += Timer_Elapsed;
        }

        private Apis.Program _NowProgram;
        public Apis.Program NowProgram
        {
            get
            {
                return _NowProgram;
            }
            set
            {
                SetProperty(ref _NowProgram, value);
            }
        }
        private void GetPrograms(int serviceID)
        {
            this.ProgramsBank = new List<Apis.Program>( services.GetPrograms(serviceID:serviceID));
        }

        public FlowDocumentScrollViewer Description
        {
            get
            {
                if (this.NowProgram == null) return null;   
                var tmp = new FlowDocument();
                var title = new Paragraph();
                title.Inlines.Add(new Bold(new Run(this.NowProgram.name)));
                title.FontSize = 20;
                tmp.Blocks.Add(title);

                var station = new Paragraph();
                station.Inlines.Add(new Italic(new Run(this.Selected.name)));
                station.FontSize = 10;
                tmp.Blocks.Add(station);

                var descript = new Paragraph();
                descript.Inlines.Add(new Italic(new Run(this.NowProgram.description)));
                descript.FontSize = 12;
                tmp.Blocks.Add(descript);
                if(this.NowProgram.extended!= null) { 
                    foreach(var i in this.NowProgram.extended)
                    {
                        var caption = new Paragraph();
                        caption.Inlines.Add(new Bold(new Run(i.Key)));
                        caption.FontSize = 14;
                        var inlines = new Paragraph();
                        inlines.Inlines.Add(new Run(i.Value));
                        inlines.FontSize = 12;
                        tmp.Blocks.Add(caption);
                        tmp.Blocks.Add(inlines);
                    }
                }
                if(this.NowProgram.video!= null) tmp.Blocks.Add(this.NowProgram.video.GetBlock());
                if (this.NowProgram.audio != null) tmp.Blocks.Add(this.NowProgram.audio.GetBlock());
                FlowDocumentScrollViewer box = new FlowDocumentScrollViewer();
                box.Document = tmp;
                return box;
            }
        }
    }
}
