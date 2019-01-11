using System.Windows.Forms;
using System.Windows.Input;

namespace RVMCore.MasterView
{ 
    public class SettingViewModel : ViewModelBase
    {
        private SettingObj data { get; set; }

        public SettingViewModel()
        {
            data = SettingObj.Read();
        }

        public DialogResult DialogResult
        {
            get; private set;
        }

        public string RootFolder
        {
            get => data.StorageFolder;
            set
            {
                if (!value.IsNullOrEmptyOrWhiltSpace()) data.StorageFolder = value;
            }
        }

        public ICommand BrowseFile
        {
            get
            {
                return new CustomCommand(() => {
                    using (OpenFileDialog dialog = new OpenFileDialog())
                    {
                        {
                            var withBlock = dialog;
                            withBlock.Filter = "Folders|*.";
                            withBlock.AddExtension = false;
                            withBlock.CheckFileExists = false;
                            withBlock.RestoreDirectory = true;
                            withBlock.Title = "Select record root Folder.";
                            withBlock.FileName = "Select Folder";
                        }
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string mPath = dialog.FileName;
                            RootFolder = mPath.EndsWith("Select Folder") ? mPath.Remove(mPath.Length - 13, 13) : mPath;
                        }
                    }
                });
            }
        }

        public bool AllowBeep
        {
            get => data.AllowBeep;
            set => data.AllowBeep = value;
        }

        public bool AllowRecordOnRoot
        {
            get=> data.AllowStoreOnBaseFolderIfTagIsNull;
            set => data.AllowStoreOnBaseFolderIfTagIsNull = value;
        }

        public string GenreNews
        {
            get => data.FolderTag_News;
            set => data.FolderTag_News = value;
        }
        public string GenreSport
        {
            get => data.FolderTag_Sports;
            set => data.FolderTag_Sports = value;
        }
        public string GenreDrama
        {
            get => data.FolderTag_Drama;
            set => data.FolderTag_Drama = value;
        }
        public string GenreMusic
        {
            get => data.FolderTag_Music;
            set => data.FolderTag_Music = value;
        }
        public string GenreVariety
        {
            get => data.FolderTag_Variety;
            set => data.FolderTag_Variety = value;
        }
        public string GenreMovie
        {
            get => data.FolderTag_Movie;
            set => data.FolderTag_Movie = value;
        }
        public string GenreAnime
        {
            get => data.FolderTag_Anime;
            set => data.FolderTag_Anime = value;
        }
        public string GenreInfo
        {
            get => data.FolderTag_Info;
            set => data.FolderTag_Info = value;
        }
        public string GenreDocum
        {
            get => data.FolderTag_Docum;
            set => data.FolderTag_Docum = value;
        }
        public string GenreLive
        {
            get => data.FolderTag_Live;
            set => data.FolderTag_Live = value;
        }
        public string GenreEdu
        {
            get => data.FolderTag_Edu;
            set => data.FolderTag_Edu = value;
        }
        public string GenreOther
        {
            get => data.FolderTag_Other;
            set => data.FolderTag_Other = value;
        }

        public string EPGServerAddr
        {
            get => data.EPG_ServiceAddr;
            set => data.EPG_ServiceAddr = value; //Need to verify data.
        }
        public string EPGUsername
        {
            get => data.EPG_UserName;
            set => data.EPG_UserName = value;
        }
        public string EPGPasswd
        {
            get => data.EPG_Passwd;
            set => data.EPG_Passwd = value;
        }
        public string EPGFolder
        {
            get => data.EPG_BaseFolder;
            set => data.EPG_BaseFolder = value;
        }

        public string MirakurunServerAddr
        {
            get => data.Mirakurun_ServiceAddr;
            set => data.Mirakurun_ServiceAddr = value; //Need to verify data.
        }

        public ICommand SaveObj => new CustomCommand(()=>
        {
            this.data.Save();
            this.DialogResult = DialogResult.OK;
        });
        

        public ICommand ResetObj => new CustomCommand(() => 
            { data = SettingObj.Read(); });

    }
}
