using System;
using System.Runtime.Serialization;

namespace RVMCore
{
    [DataContract]
    public class SettingObj
    {
        private const string @DefaultStr = "%%AppBase%%";
        internal static readonly string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting.json");
        [DataMember]
        private string @BaseFolder = DefaultStr;
        public string StorageFolder
        {
            get
            {
                if (BaseFolder.Contains(DefaultStr))
                    return BaseFolder.Replace(DefaultStr, AppDomain.CurrentDomain.BaseDirectory);
                else
                    return BaseFolder;
            }
            set
            {
                if (value.Contains(AppDomain.CurrentDomain.BaseDirectory))
                    BaseFolder = value.Replace(AppDomain.CurrentDomain.BaseDirectory, DefaultStr);
                else
                    BaseFolder = value;
            }
        }
        [DataMember]
        public bool AllowStoreOnBaseFolderIfTagIsNull = false;
        [DataMember]
        public string FolderTag_Drama = "ドラマ類";
        [DataMember]
        public string FolderTag_Anime = "アニメ類";
        [DataMember]
        public string FolderTag_Variety = "バラエティー類";
        [DataMember]
        public string FolderTag_Movie = "映画類";
        [DataMember]
        public string FolderTag_Docum = "ドキュメント類";
        [DataMember]
        public string FolderTag_Live = "公演類";
        [DataMember]
        public string FolderTag_Other;
        [DataMember]
        public string FolderTag_News;
        [DataMember]
        public string FolderTag_Sports;
        [DataMember]
        public string FolderTag_Edu;
        [DataMember]
        public string FolderTag_Music;
        [DataMember]
        public string FolderTag_Info;
        [DataMember]
        public bool AllowBeep = true;
        [DataMember]
        public string EPG_UserName="Look";
        [DataMember]
        public string EPG_Passwd="me";
        [DataMember]
        public string EPG_ServiceAddr = "localhost:40888";
        [DataMember]
        public string EPG_BaseFolder = "Cache";
        [DataMember]
        public string Mirakurun_ServiceAddr = "http://localhost:40772/";
        public string GetFolderTag(ProgramGenre ge)
        {
            string myResult = string.Empty;
            switch (ge)
            {
                case ProgramGenre.Drama:
                    {
                        myResult = FolderTag_Drama;
                        break;
                    }

                case ProgramGenre.Anime:
                    {
                        myResult = FolderTag_Anime;
                        break;
                    }

                case ProgramGenre.Variety:
                    {
                        myResult = FolderTag_Variety;
                        break;
                    }

                case ProgramGenre.Movie:
                    {
                        myResult = FolderTag_Movie;
                        break;
                    }

                case ProgramGenre.Documantry:
                    {
                        myResult = FolderTag_Docum;
                        break;
                    }

                case ProgramGenre.Live:
                    {
                        myResult = FolderTag_Live;
                        break;
                    }

                case ProgramGenre.News:
                    {
                        myResult = FolderTag_News;
                        break;
                    }

                case ProgramGenre.Sports:
                    {
                        myResult = FolderTag_Sports;
                        break;
                    }

                case ProgramGenre.Education:
                    {
                        myResult = FolderTag_Edu;
                        break;
                    }

                case ProgramGenre.Music:
                    {
                        myResult = FolderTag_Music;
                        break;
                    }

                case ProgramGenre.Infomation:
                    {
                        myResult = FolderTag_Info;
                        break;
                    }

                default:
                    {
                        myResult = FolderTag_Other;
                        break;
                    }
            }
            if (string.IsNullOrWhiteSpace(myResult))
                return AllowStoreOnBaseFolderIfTagIsNull ? myResult : string.IsNullOrWhiteSpace(FolderTag_Other) ? "その他番組" : FolderTag_Other;
            else
                return myResult;
        }

        public void Save()
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Move(path, path + ".old");
            }
            System.Runtime.Serialization.Json.DataContractJsonSerializer sr = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SettingObj));
            using (System.IO.FileStream sw = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate))
            {
                try
                {
                    sr.WriteObject(sw, this);
                }
                catch (System.Exception ex)
                {
                    if (System.IO.File.Exists(path + ".old"))
                        System.IO.File.Move(path + ".old", path);
                    System.Windows.Forms.MessageBox.Show(ex.Message,"Error while writting setting files.");
                }
            }
            System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) | System.IO.FileAttributes.Hidden);
            if (System.IO.File.Exists(path + ".old"))
                System.IO.File.Delete(path + ".old");
        }

        public static SettingObj Read()
        {
            if (!System.IO.File.Exists(path))
            {
                SettingObj tmp = new SettingObj();
                tmp.Save();
                return tmp;
            }
            System.Runtime.Serialization.Json.DataContractJsonSerializer sr = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SettingObj));
            using (System.IO.FileStream sw = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                return (SettingObj)sr.ReadObject(sw);
            }
        }
    }

}
