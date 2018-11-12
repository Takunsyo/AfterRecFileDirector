using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RVMCore.Forms;

namespace RVMCore.MirakurunWarpper
{
    class MirakurunViewerView : ViewModelBase
    {

        public MirakurunViewerView()
        {
            services = new MirakurunService("http://127.0.0.1:40772/");
            _ChannelList = new Dictionary<ChannelType, ObservableCollection<Apis.Service>>();
            _ChannelList.Add(ChannelType.GR, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.GR)));
            _ChannelList.Add(ChannelType.BS, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.BS)));
            _ChannelList.Add(ChannelType.CS, new ObservableCollection<Apis.Service>(services.GetServices(cType: ChannelType.CS)));
            _ChannelList.Add(ChannelType.SKY, new ObservableCollection<Apis.Service>());
        }
        private MirakurunService services;
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

        public void ChangeChannel()
        {
            this.ViewUri =new Uri(services.GetServiceStreamPath(this.Selected.id));
            NotifyPropertyChanged("ViewUri");
        }
    }
}
