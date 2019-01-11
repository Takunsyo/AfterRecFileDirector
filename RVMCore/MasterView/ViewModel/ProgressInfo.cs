namespace RVMCore.MasterView.ViewModel
{

    public class ProgressInfo : ViewModelBase
    {
        public string Text
        {
            get
            {
                if (max == val) return "" + Extra;
                return "[" + getSizeString(val) + "/" + getSizeString(max) + "]" + Extra;
            }
        }
        private ulong max;
        public int Max
        {
            get => (int)(max / 256);
        }
        private ulong val;
        public int Val
        {
            get => (int)(val / 256);
        }
        public string Extra { get; set; }
        private string getSizeString(ulong size)
        {
            string tmp = "";
            if (size > 1024 * 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024 / 1024).ToString("F2") + " Gb";
            }
            else if (size > 1024 * 512)
            {
                tmp = ((float)size / 1024 / 1024).ToString("F2") + " Mb";
            }
            else if (size > 1024)
            {
                tmp = ((float)size / 1024).ToString("F2") + " Kb";
            }
            else
            {
                tmp = (size).ToString() + " Byte";
            }
            return tmp;
        }
        public void SetValue(int val, string extra)
        {
            this.val = (ulong)val;
            this.Extra = extra;
            this.NotifyPropertyChanged("Extra");
            this.NotifyPropertyChanged("Val");
            this.NotifyPropertyChanged("Text");
        }
        public void SetValue(int val, int max, string extra)
        {
            this.val = (ulong)val;
            this.max = (ulong)max;
            this.Extra = extra;
            this.NotifyPropertyChanged("Val");
            this.NotifyPropertyChanged("Max");
            this.NotifyPropertyChanged("Extra");
            this.NotifyPropertyChanged("Text");
        }
        public void SetValue(long val, string extra)
        {
            this.val = (ulong)val;
            this.Extra = extra;
            this.NotifyPropertyChanged("Val");
            this.NotifyPropertyChanged("Extra");
            this.NotifyPropertyChanged("Text");
        }
        public void SetValue(long val, long max, string extra)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.Extra = extra;
            this.NotifyPropertyChanged("Max");
            this.NotifyPropertyChanged("Val");
            this.NotifyPropertyChanged("Extra");
            this.NotifyPropertyChanged("Text");
        }
        public void SetValue(long val, long max, long extra)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.Extra = getSizeString((ulong)extra) + @"/s";
            this.NotifyPropertyChanged("Extra");
            this.NotifyPropertyChanged("Text");
            this.NotifyPropertyChanged("Max");
            this.NotifyPropertyChanged("Val");
        }
        public void SetValue(int val, int max, int extra)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.Extra = getSizeString((ulong)extra) + @"/s";
            this.NotifyPropertyChanged("Extra");
            this.NotifyPropertyChanged("Text");
            this.NotifyPropertyChanged("Max");
            this.NotifyPropertyChanged("Val");
        }
        public void SetValue(int val, int max)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.NotifyPropertyChanged("Max");
            this.NotifyPropertyChanged("Text");
            this.NotifyPropertyChanged("Val");
        }
        public void SetValue(long val, long max)
        {
            this.max = (ulong)max;
            this.val = (ulong)val;
            this.NotifyPropertyChanged("Max");
            this.NotifyPropertyChanged("Text");
            this.NotifyPropertyChanged("Val");
        }
        public ProgressInfo()
        {
            this.val = 0;
            this.max = 100;
            this.Extra = "";
        }

    }

}
