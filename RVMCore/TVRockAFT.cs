using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using RVMCore.EPGStationWarpper;
using System.Windows.Forms.Integration;

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
            "App started.".InfoLognConsole();
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
                    "Fail to read settings [{0}]".InfoLognConsole(ex.Message);
                    Console.WriteLine("Sleep 10 sec...");
                    System.Threading.Tasks.Task.Delay(10000);
                }
            }
            EPGAccess mAccess = null;
            StreamFile mpars = null;
            if (margs.Any(x => x.Equals("-main", StringComparison.OrdinalIgnoreCase)))
            {
                var wpfwindow = new MasterView.MasterViewControl();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                if (wpfwindow.ShowDialog() == true) return true;
            }
            if (margs.Any(x => x.Equals("-mirakurun", StringComparison.OrdinalIgnoreCase)))
            {
                var wpfwindow = new MirakurunWarpper.MirakurunViewer();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                if (wpfwindow.ShowDialog() == true) return true;
            }
            if (margs.Any(x => x.Equals("-setup", StringComparison.OrdinalIgnoreCase)))
            {
                var wpfwindow = new MasterView.Setting();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                if (wpfwindow.ShowDialog() == true) return true;
            }
            if (margs.Any(x => x.Equals("-rcdbview", StringComparison.OrdinalIgnoreCase)))
            {
                var wpfwindow = new MasterView.RecordedListView();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                if (wpfwindow.ShowDialog() == true) return true;
            }
            if (margs.Any(x => x.Equals("-cloud", StringComparison.OrdinalIgnoreCase)))
            {
                var wpfwindow = new MasterView.CloudViewer();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                if (wpfwindow.ShowDialog() == true) return true;
            }
            if (margs.Any(x => x.Equals("-upload", StringComparison.OrdinalIgnoreCase)))
            {
                var wpfwindow = new MasterView.Uploader();
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                if (wpfwindow.ShowDialog() == true) return true;
            }
            if (margs.Any(x => x.Equals("-epgstation", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=EPGstation=-=-=-=-=-=-=-=-=-=-=-");
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
                "App start with parameter\"{0}\"".InfoLognConsole(sbb.ToString());
                clPara.Add(sbb.ToString()); // Lack of the last run in the loop, so add the last parameter manuly.
                int id = -1;
                bool t_check= false;
                try
                {//first see para -id 
                    t_check = int.TryParse(clPara.First(x => x.StartsWith("-id")).Substring(4), out id);
                }
                catch
                {
                    t_check = false;
                }
                if (!t_check)
                {//if -id is not working, try get enviroment variable. maybe not working.
                    "parameter -id not found or not a number.".ErrorLognConsole();
                    var e_id = Environment.GetEnvironmentVariable("RECORDEDID");
                    if (!e_id.IsNullOrEmptyOrWhiltSpace()&& !int.TryParse(e_id, out id))
                    {
                        "Failed to find \"RECORDEDID\" from Enviorment variable".ErrorLognConsole();                    
                        "App catch error. exiting...".InfoLognConsole();
                        return false;
                    }
                    "Environment Variable \"RECORDEDID={0}\"".InfoLognConsole(id.ToString());
                }
                Console.WriteLine("Preparing for access Epgstation server.");
                mAccess = new EPGAccess(mySetting); 
                mpars = mAccess.GetStreamFileObj(id);
                if (mpars == null)
                {
                    "Remote file is missing or \"ID:{0}\" does not exsits.".ErrorLognConsole( id.ToString());
                    "App catch error. exiting...".InfoLognConsole();
                    return false;
                }
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
            if (!MoveFile(mpars, mySetting,mAccess))
            {
                "Local file is missing : \"File:{0}\" does not exsit.".ErrorLognConsole(System.IO.Path.GetFileName(mpars.FilePath));
                "App catch error. exiting...".InfoLognConsole();
                return false;
            };
            "App has completed job. exiting...".InfoLognConsole();
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

        private static bool MoveFile(StreamFile para, SettingObj mySetting,EPGAccess epgAccess = null)
        {
            if (!System.IO.File.Exists(para.FilePath)) return false; // Ops! file is not there.
            string fileName = System.IO.Path.GetFileName(para.FilePath);
            string[] FolderList = System.IO.Directory.GetDirectories(mySetting.StorageFolder);//Get local preset base folder's child folers.
            string Targetfolder = mySetting.StorageFolder;
            RmtFile mFile = new RmtFile();  // Object for Upload process
            if (!string.IsNullOrWhiteSpace(mySetting.GetFolderTag(para.Genre))) 
            {
                try //try get full path of genre folder.
                {
                    Targetfolder = FolderList.First(x => Strings.InStr(x, mySetting.GetFolderTag(para.Genre)) > 0);
                }
                catch (Exception e)// In case the preset genre folder is not there.
                {
                    Targetfolder = System.IO.Path.Combine(mySetting.StorageFolder, mySetting.GetFolderTag(para.Genre));
                    Console.WriteLine(e.Message);
                }
            }
            if (!System.IO.Directory.Exists(Targetfolder)) //if genre folder is not there make one.
                System.IO.Directory.CreateDirectory(Targetfolder);
            if (para.Genre.HasFlag(ProgramGenre.Anime) || para.Genre.HasFlag(ProgramGenre.Drama) || para.Genre.HasFlag(ProgramGenre.Variety))
            { // those programs has genre of anime drama or variety could be in series. if that is the case make a folder to hold them.
                string programName = MasterHelper.FindTitle(para.Title); //find title.
                if (!programName.Equals(para.Title)) // if the title find by program doesn't match it's full name means it's in a series.
                {
                    FolderList = System.IO.Directory.GetDirectories(Targetfolder);
                    try // from here on is basicly copy plast the folder exist thing.
                    {
                        Targetfolder = FolderList.First(x =>
                        { // find a folder matchs series name
                            var str = x.Substring(x.LastIndexOf(@"\") + 1);
                            str = str.Substring(str.IndexOf("]") + 1);
                            return Strings.InStr(x, programName) > 0 ||
                            Strings.InStr(x, Strings.StrConv(programName, VbStrConv.Wide)) > 0 ||
                            Strings.InStr(programName, str) > 0 ||
                            Strings.InStr(Strings.StrConv(programName, VbStrConv.Wide), str) > 0;
                        });
                    }
                    catch (Exception e)
                    { //no match so far so make a one.
                        Targetfolder = System.IO.Path.Combine(Targetfolder, MasterHelper.GetTimeSpan(para.StartTime, para.StartTime) + programName);
                        Console.WriteLine("Try find folder : " + e.Message);
                    }
                    mFile.FullFilePath = System.IO.Path.Combine(Targetfolder, fileName);
                    //this will make sure the date period at the head of folder name is correct.
                    if (MasterHelper.RenameDirUpToDate(ref Targetfolder, para.EndTime))
                    {//this is for upload process . notify that the folder name has been changed.
                        mFile.OldFatherName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(mFile.FullFilePath));
                        mFile.IsFatherUpdate = true;
                        mFile.FullFilePath = System.IO.Path.Combine(Targetfolder, fileName);
                    }
                    Console.WriteLine("Target folder is : " + Targetfolder);
                    if (!System.IO.Directory.Exists(Targetfolder))
                    {
                        System.IO.Directory.CreateDirectory(Targetfolder);
                        "Create folder: {0}".InfoLognConsole( Targetfolder);
                    }
                }
                //Move file to where it belongs. 
                FileMovier(para.FilePath, System.IO.Path.Combine(Targetfolder, fileName));
                para.FilePath = System.IO.Path.Combine(Targetfolder, fileName);
                if (epgAccess == null)
                {// if the epgAccess is null means this process is called by tvrock 
                    //no need to get extra information from server.
                    //XML file is standerd meta data format for tvrock for now.
                    para.ToXml(System.IO.Path.Combine(Targetfolder, fileName));
                }
                else
                {// this process is called by epgstation or else.
                    //information is from server, it will be stored in a 
                    //*.meta file, it could also include a station logo and a thumbnail of video.
                    var tmp = para.EPGStation.WtiteFile(System.IO.Path.Combine(Targetfolder, fileName));
                    if (tmp) epgAccess.DeleteRecordByID(para.EPGStation.Meta.id);
                }
                //comit upload
                AddToDatabase(mFile,mySetting,para.StartTime);
                OKBeep(mySetting); // beep
            }
            else
            {
                mFile.FullFilePath = System.IO.Path.Combine(Targetfolder, fileName);
                FileMovier(para.FilePath, System.IO.Path.Combine(Targetfolder, fileName));
                para.FilePath = System.IO.Path.Combine(Targetfolder, fileName);
                if (epgAccess==null )
                {
                    para.ToXml(System.IO.Path.Combine(Targetfolder, fileName));
                }
                else
                {
                    var tmp = para.EPGStation.WtiteFile(System.IO.Path.Combine(Targetfolder, fileName));
                    if (tmp) epgAccess.DeleteRecordByID(para.EPGStation.Meta.id);
                }
                //Do upload same here..
                AddToDatabase(mFile, mySetting, para.StartTime);
                OKBeep(mySetting);
            }
            return true;
        }
        /// <summary>
        /// Upload method *unstable* under test.
        /// </summary>
        /// <param name="file"><see cref="RmtFile"/> object.</param>
        /// <param name="counter">fail counter.</param>
        private static void Upload(RmtFile file, int counter = -1)
        {
            if (counter >= 3) "Failed to startup or connact to upload instance!".ErrorLognConsole();
            var client = new RVMCore.PipeClient<RmtFile>();
            for (int i = 0; i <= 3; i++)
            { // try up to 4 times.
                try
                {
                    client.Send(file, "RVMCoreUploader");
                    return; // if success return.
                }
                catch (Exception ex)
                {
                    System.Threading.Tasks.Task.Delay(1000);
                    ex.Message.ErrorLognConsole();
                }
            }
            // if it come to here. then there should be not a upload instance present. so strat one.
            int pid = 0;
            try
            {
                // this ProcessExtensions should help startup a instance to a user... ummmm..untested.
                ProcessExtensions.StartProcessAsCurrentUser(
                    System.Reflection.Assembly.GetEntryAssembly().Location, out pid);
                "Uploader process has been lanchued, PID[{0}]".InfoLognConsole(pid);
            }
            catch(Exception ex)
            {
                "Failed to start upload process [{0}]".ErrorLognConsole(ex.Message);
            }
            System.Threading.Tasks.Task.Delay(2000);
            counter += 1; //Pass fail to counter. then run from first line.
            Upload(file, counter);
        }

        private static void AddToDatabase(RmtFile file, SettingObj setting,DateTime startTime)
        {
            try
            {
                if (!setting.DataBase.Equals("mysql", StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidOperationException("Database is wrone or not in use.");
                using (var database = new Database(setting.DataBase_Addr, setting.DataBase_User, setting.DataBase_Pw))
                {
                    database.AddDataItem(file.FullFilePath.Substring(file.FullFilePath.IndexOf('\\') + 1),
                        new System.IO.FileInfo(file.FullFilePath).Name,
                        MirakurunWarpper.MirakurunService.GetUNIXTimeStamp(startTime),
                        file.IsFatherUpdate ? file.OldFatherName : null);
                }
            }
            catch(Exception ex)
            {
                ex.Message.ErrorLognConsole();
                Upload(file);
            }
        }


        public static void TestMethod()
        {
            //var path = @"E:\AfterRecFileDirector.exe -upload";
            //ProcessExtensions.StartProcessAsCurrentUser(path, out var pid);
            RVMCore.GoogleWarpper.GoogleDrive googleDrive = new RVMCore.GoogleWarpper.GoogleDrive();
            string uri = "";
            string path = @"D:\迅雷下载\1.Bank\Learning\新求精德语初级1_mp3.rar";
            string getUri(bool force)
            {
                if (string.IsNullOrWhiteSpace(uri) || force)
                {
                    uri = googleDrive.GenerateUploadID(path, @"\");
                }
                return uri;
            }
            googleDrive.UploadResumableAsync(path, getUri);
        }

        private static bool FileMovier(string old , string tar)
        {
            bool check = false;
            do
            { //Check untill the file is accessible.
                System.IO.FileStream stream = null;
                try
                {
                    stream = System.IO.File.Open(old, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    check = true;
                }
                catch (System.IO.IOException)
                {
                    check = false;
                    System.Threading.Tasks.Task.Delay(200);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            } while (!check);

            try
            {
                "Moving file to: {0}".InfoLognConsole( System.IO.Path.GetDirectoryName(tar));
                System.IO.File.Move(old, tar);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! " + e.Message);
                e.Message.ErrorLognConsole();
                return false;
            }
        }

        private static void OKBeep(SettingObj mySetting)
        {
            if (!mySetting.AllowBeep)
                return;
            // Beep in morse says OK
            Console.Beep(550, 200);
            System.Threading.Tasks.Task.Delay(20);
            Console.Beep(550, 200);
            System.Threading.Tasks.Task.Delay(20);
            Console.Beep(550, 200);
            System.Threading.Tasks.Task.Delay(50);
            Console.Beep(550, 200);
            System.Threading.Tasks.Task.Delay(20);
            Console.Beep(550, 100);
            System.Threading.Tasks.Task.Delay(10);
            Console.Beep(550, 200);
            System.Threading.Tasks.Task.Delay(20);
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


    }
}
