using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using RVMCore;
using Newtonsoft.Json;
using System.Net;

namespace RVMCore
{
    public static class TVAFT
    {
        public static bool IsNullOrEmptyOrWhiltSpace(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }
        public static bool SortFile(string[] margs)
        {
            SettingObj mySetting = null;
            bool a = false;
            while (!a)
            {
                Console.WriteLine("Reading settings.");

                try
                {
                    mySetting = SettingObj.Read();                    
                    //mySetting = ((dynamic)Activator.CreateInstance(SettingObj)).Read();
                    a = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error : {0}", ex.Message);
                    Console.WriteLine("Sleep 10 sec...");
                    System.Threading.Thread.Sleep(10000);
                }
            }
            StreamFile mpars = null;
            if (margs.Contains("-epgstation")|| margs.Contains("-EPGSTATION"))
            {
                // Start process the parameters, group them to their own entries.
                List<string> clPara = new List<string>();
                if (margs.Count() <= 1)
                    return false;/* TODO Change to default(_) if this is not a reference type */
                System.Text.StringBuilder sbb = new System.Text.StringBuilder();
                foreach (var p in margs)
                {
                    if (p.StartsWith("-"))
                    {
                        var tmp = sbb.ToString();
                        if (tmp.Length > 1 && tmp.StartsWith("-"))
                            clPara.Add(tmp);
                        sbb.Clear(); // Clear the stringbuilder every time it hits the char "-"
                        sbb.Append(p);
                    }
                    else
                        sbb.AppendFormat(" {0}", p);
                }
                clPara.Add(sbb.ToString()); // Lack of the last run in the loop, so add the last parameter manuly.
                var id = int.Parse(clPara.First(x => x.StartsWith("-id")).Substring(4));
                mpars = GetEPGFileInfo(id, mySetting.EPG_UserName, mySetting.EPG_Passwd,mySetting.EPG_ServiceAddr, System.IO.Path.Combine( mySetting.StorageFolder,mySetting.EPG_BaseFolder));
                if (mpars.ChannelName.IsNullOrEmptyOrWhiltSpace())
                {
                    string channel = "";
                    try
                    {
                        channel = clPara.First(x => x.StartsWith("-cn")).Substring(4);
                    }
                    catch
                    {
                        channel = "UNKNOW";
                    }
                    finally
                    {
                        mpars.ChannelName = channel;
                    }
                }
            }
            else
            {
                mpars = GetPara(margs);
            }
            if (mpars == null)
                return false;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var p in margs)
                sb.Append(p + " ");
            PrintInfomation(mpars);
            MoveFile(mpars,mySetting);
            // Leave a log. Assembly.GetEntryAssembly().Location Environment.CurrentDirectory
            string logPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!System.IO.Directory.Exists(logPath))
                System.IO.Directory.CreateDirectory(logPath);
            using (System.IO.StreamWriter s = new System.IO.StreamWriter(System.IO.Path.Combine(logPath, mpars.ID.ToString() + ".log"), false))
            {
                s.WriteLine(sb.ToString());
            }
            return true;
        }

        private static void PrintInfomation(StreamFile paras)//private static void PrintInfomation(StreamFile paras)
        {
            if (paras == null)
                return;
            Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-= File Info =-=-=-=-=-=-=-=-=-=-=-");
            Console.WriteLine("File:         {0}", paras.FilePath);
            Console.WriteLine("Genre:        {0}", paras.Genre);
            Console.WriteLine("Title:        {0}", paras.Title);
            Console.WriteLine("recTitle:     {0}", paras.recTitle);
            Console.WriteLine("Subtitle:     {0}", paras.recSubTitle);
            Console.WriteLine("Channel:      {0}", paras.ChannelName);
            Console.WriteLine("Content:      {0}", paras.Content);
            Console.WriteLine("Infomation:   {0}", paras.Infomation);
            Console.WriteLine("StartTime:    {0}", paras.StartTime);
            Console.WriteLine("EndTime:      {0}", paras.EndTime);
            Console.WriteLine("SerachKeyword:{0}", paras.recKeyWord);
            Console.WriteLine("KeywordInfo:  {0}", paras.recKeywordInfo);
            Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
        }

        private static StreamFile GetPara(string[] paras)//private static StreamFile GetPara(string[] paras)
        {
            // Start process the parameters, group them to their own entries.
            List<string> clPara = new List<string>();
            if (paras.Count() <= 1)
                return null/* TODO Change to default(_) if this is not a reference type */;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var p in paras)
            {
                if (p.StartsWith("-"))
                {
                    var tmp = sb.ToString();
                    if (tmp.Length > 1 && tmp.StartsWith("-"))
                        clPara.Add(tmp);
                    sb.Clear(); // Clear the stringbuilder every time it hits the char "-"
                    sb.Append(p);
                }
                else
                    sb.AppendFormat(" {0}", p);
            }
            clPara.Add(sb.ToString()); // Lack of the last run in the loop, so add the last parameter manuly.

            Console.WriteLine();
            StreamFile mPara = new StreamFile();
            foreach (var p in clPara)
            {
                var i = p.Substring(1, 1);
                switch (i.ToLower())
                {
                    case "i":
                        {
                            string m = p.Substring(p.IndexOf(" ") + 1);
                            if (System.IO.File.Exists(m))
                                mPara.FilePath = m;
                            else
                            { Console.WriteLine("Input file could not found."); return null/* TODO Change to default(_) if this is not a reference type */; }
                            break;
                        }
                    case "g":
                        {
                            string m = p.Substring(p.IndexOf(" ") + 1);
                            mPara.Genre = GetGenre(m);
                            break;
                        }
                    case "d":
                        {
                            if (p.Length > 3)
                                mPara.Title = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "c":
                        {
                            if (p.Length > 3)
                                mPara.ChannelName = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "e":
                        {
                            if (p.Length > 3)
                                mPara.Content = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "f":
                        {
                            if (p.Length > 3)
                                mPara.Infomation = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "k":
                        {
                            if (p.Length > 3)
                            {
                                string m = p.Substring(p.IndexOf(" ") + 1);
                                mPara.StartTime = DateTime.Parse(m);
                            }
                            break;
                        }
                    case "l":
                        {
                            if (p.Length > 3)
                            {
                                string m = p.Substring(p.IndexOf(" ") + 1);
                                mPara.EndTime = DateTime.Parse(m);
                            }
                            break;
                        }
                    case "9":
                        {
                            if (p.Length > 3)
                                mPara.recSubTitle = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "8":
                        {
                            if (p.Length > 3)
                                mPara.recTitle = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "a":
                        {
                            if (p.Length > 3)
                                mPara.recKeyWord = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    case "b":
                        {
                            if (p.Length > 3)
                                mPara.recKeywordInfo = p.Substring(p.IndexOf(" ") + 1);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Unknow paramater");
                            break;
                        }
                }
            }
            return mPara;
        }

        private static void MoveFile(StreamFile para,SettingObj mySetting)
        {
            string fileName = System.IO.Path.GetFileName(para.FilePath);
            string[] FolderList = System.IO.Directory.GetDirectories(mySetting.StorageFolder);
            string Targetfolder = mySetting.StorageFolder;
            if (!string.IsNullOrWhiteSpace(mySetting.GetFolderTag(para.Genre)))
            {
                try // In case you dont find it or it does not exist
                {
                    Targetfolder = FolderList.First(x => Strings.InStr(x, mySetting.GetFolderTag(para.Genre)) > 0);
                }
                catch (Exception e)
                {
                    Targetfolder = System.IO.Path.Combine(mySetting.StorageFolder, mySetting.GetFolderTag(para.Genre));
                    Console.WriteLine(e.Message);
                }
            }
            if (!System.IO.Directory.Exists(Targetfolder))
                System.IO.Directory.CreateDirectory(Targetfolder);
            if (para.Genre.HasFlag(ProgramGenre.Anime) || para.Genre.HasFlag(ProgramGenre.Drama) || para.Genre.HasFlag(ProgramGenre.Variety))
            {
                string programName = Share.FindTitle(para.Title);
                if (!programName.Equals(para.Title))
                {
                    FolderList = System.IO.Directory.GetDirectories(Targetfolder);
                    try // from here on is basicly copy plast the folder exist thing.
                    {
                        Targetfolder = FolderList.First(x =>
                        {
                            var str = x.Substring(x.LastIndexOf(@"\") + 1);
                            str = str.Substring(str.IndexOf("]") + 1);
                            return Strings.InStr(x, programName) > 0 ||
                            Strings.InStr(x, Strings.StrConv(programName, VbStrConv.Wide)) > 0 ||
                            Strings.InStr(programName, str) > 0 ||
                            Strings.InStr(Strings.StrConv(programName, VbStrConv.Wide), str) > 0;
                        });
                    }
                    catch (Exception e)
                    {
                        Targetfolder = System.IO.Path.Combine(Targetfolder, Share.GetTimeSpan(para.StartTime, para.StartTime) + programName);
                        Console.WriteLine("Try find folder : " + e.Message);
                    }

                    Share.RenameDirUpToDate(ref Targetfolder, para.EndTime); // Here corrects the date things.
                    Console.WriteLine("Target folder is : " + Targetfolder);
                    if (!System.IO.Directory.Exists(Targetfolder))
                        System.IO.Directory.CreateDirectory(Targetfolder);
                }
                try
                {
                    System.IO.File.Move(para.FilePath, System.IO.Path.Combine(Targetfolder, fileName));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! " + e.Message);
                }
                para.FilePath = System.IO.Path.Combine(Targetfolder, fileName);
                para.ToXml(System.IO.Path.Combine(Targetfolder, fileName));
                OKBeep(mySetting);
            }
            else
            {
                try
                {
                    System.IO.File.Move(para.FilePath, System.IO.Path.Combine(Targetfolder, fileName));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! " + e.Message);
                }
                para.FilePath = System.IO.Path.Combine(Targetfolder, fileName);
                para.ToXml(System.IO.Path.Combine(Targetfolder, fileName));
                OKBeep(mySetting);
            }
        }

        private static void OKBeep(SettingObj mySetting)
        {
            if (!mySetting.AllowBeep)
                return;
            // Beep in morse says OK
            Console.Beep(550, 200);
            System.Threading.Thread.Sleep(20);
            Console.Beep(550, 200);
            System.Threading.Thread.Sleep(20);
            Console.Beep(550, 200);
            System.Threading.Thread.Sleep(50);
            Console.Beep(550, 200);
            System.Threading.Thread.Sleep(20);
            Console.Beep(550, 100);
            System.Threading.Thread.Sleep(10);
            Console.Beep(550, 200);
            System.Threading.Thread.Sleep(20);
        }

        private static ProgramGenre GetGenre(string gen)
        {
            if (gen.Contains("ニュース") || gen.Contains("報道"))
                return ProgramGenre.News;
            if (gen.Contains("スポーツ"))
                return ProgramGenre.Sports;
            if (gen.Contains("ドラマ"))
                return ProgramGenre.Drama;
            if (gen.Contains("音楽"))
                return ProgramGenre.Music;
            if (gen.Contains("バラエティー"))
                return ProgramGenre.Variety;
            if (gen.Contains("映画"))
                return ProgramGenre.Movie;
            if (gen.Contains("アニメ") || gen.Contains("特撮"))
                return ProgramGenre.Anime;
            if (gen.Contains("情報") || gen.Contains("ワイドショー"))
                return ProgramGenre.Infomation;
            if (gen.Contains("ドキュメンタリー"))
                return ProgramGenre.Documantry;
            if (gen.Contains("劇場") || gen.Contains("公演"))
                return ProgramGenre.Live;
            if (gen.Contains("趣味") || gen.Contains("教育"))
                return ProgramGenre.Education;
            return ProgramGenre.Default;
        }


        private class epgDefault
        {
            public int code;
            public string message;
            public string errors;
        }

        //EPGStation API
        // /recorded/{id}
        private class RecordedProgram : epgDefault
        {
            public int id;
            public long channelId;
            public string channelType;
            public long startAt;
            public long endAt;
            public string name;
            public string description;
            public string extended;
            public int genre1;
            public int genre2;
            public string videoType;
            public string videoResolution;
            public int videoStreamContent;
            public int videoComponentType;
            public int audioSamplingRate;
            public int audioComponentType;
            public bool recording;
            public bool protection;
            public long filesize;
            public int errorCnt;
            public int dropCnt;
            public int scramblingCnt;
            public bool hasThumbnail;
            public bool original;
            public string filename;
            public dynamic encoded; //not needed
            public dynamic encoding;//not needed
        }

        private class EPGRecorded: RecordedProgram
        {
            public c_type ChannelType {
                get {
                    switch (base.channelType.ToUpper())
                    {
                        case "GR":
                            return c_type.GR;
                            break;
                        case "BS":
                            return c_type.BS;
                            break;
                        case "CS":
                            return c_type.CS;
                            break;
                        case "SKY":
                            return c_type.SKY;
                            break;
                        default:
                            return c_type.UNKNOW;
                    }
                }
            }
        }
        private enum c_type
        {
            GR = 0x1,
            BS = 0x01,
            CS = 0x001,
            SKY = 0x0001,
            UNKNOW = 0x00000001
        }
        private static StreamFile GetEPGFileInfo(int id,string username,string passwd,string serviceAddr,string baseFolder)
        {
            if (id == null || id <= 0) return null;
            string EPGATTR = "http://{1}/api/recorded/{0}";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format(EPGATTR,id.ToString(),serviceAddr));
            req.Method = "GET";
            req.Credentials = new System.Net.NetworkCredential(username,passwd);
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch
            {
                return null;
            }
            RecordedProgram body;
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                byte[] buffer = new byte[int.Parse(cLen)];
                st.ReadAsync(buffer, 0, int.Parse(cLen));                
                string tmp = System.Text.Encoding.UTF8.GetString(buffer);
                body = JsonConvert.DeserializeObject<RecordedProgram>(tmp);
            }
            StreamFile mFile = new StreamFile();
            mFile.ChannelName = GetChannelNameByID(body.channelId, username, passwd, serviceAddr);
            mFile.Content = body.description;
            mFile.Infomation = body.extended;
            mFile.ID = new Guid();
            mFile.Title = body.name;
            mFile.recTitle = body.name;
            mFile.recKeyWord = "EPGStation "+body.id.ToString();
            mFile.recSubTitle = body.videoResolution + body.videoResolution + body.videoType;
            mFile.recKeywordInfo = string.Format("Error:E({0})D({1})S({2})", body.errorCnt, body.dropCnt, body.scramblingCnt);
            switch (body.genre1)
            {
                case 0: mFile.Genre = ProgramGenre.News; break;
                case 1:mFile.Genre = ProgramGenre.Sports;break;
                case 2:mFile.Genre = ProgramGenre.Infomation;break;
                case 3:mFile.Genre = ProgramGenre.Drama;break;
                case 4:mFile.Genre = ProgramGenre.Music;break;
                case 5:mFile.Genre = ProgramGenre.Variety;break;
                case 6:mFile.Genre = ProgramGenre.Movie;break;
                case 7:mFile.Genre = ProgramGenre.Anime;break;
                case 8:mFile.Genre = ProgramGenre.Documantry;break;
                case 9:mFile.Genre = ProgramGenre.Live;break;
                case 10: mFile.Genre = ProgramGenre.Education;break;
                default:mFile.Genre = ProgramGenre.Others;break;
            }
            mFile.StartTime = GetTimeFromUNIXTime(body.startAt);
            mFile.EndTime = GetTimeFromUNIXTime(body.endAt);
            mFile.FilePath = System.IO.Path.Combine(baseFolder,System.Web.HttpUtility.UrlDecode(body.filename));
            return mFile;
        }
        private static DateTime GetTimeFromUNIXTime(long time)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(time/1000);
            return dateTime;
        }
        private static string GetChannelNameByID(long cid,string username,string passwd,string serviceAddr)
        {
            string EPGATTR = "http://{0}/api/channels";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format(EPGATTR, serviceAddr));
            req.Method = "GET";
            req.Credentials = new System.Net.NetworkCredential(username, passwd);
            //req.ContentType = "application/json; charset=utf-8";
            HttpWebResponse resp;
            try
            {
                 resp = (HttpWebResponse)req.GetResponse();
            }
            catch
            {
                return null;
            }
            List<EPGchannel> body;
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                byte[] buffer = new byte[int.Parse(cLen)];
                st.ReadAsync(buffer, 0, int.Parse(cLen));
                string tmp = System.Text.Encoding.UTF8.GetString(buffer);
                body = JsonConvert.DeserializeObject<List<EPGchannel>>(tmp);
            }
            try
            {
                return body.First(x => x.id == cid).name;
            }
            catch
            {
                return null;
            }
        }
        private class EPGchannel:epgDefault
        {
            public long id;
            public int serviceId;
            public int networkId;
            public string name;
            public int remoteControlKeyId;
            public bool hasLogoData;
            public string channelType;
            public string channel;
            public int type;
        }
    }
}
