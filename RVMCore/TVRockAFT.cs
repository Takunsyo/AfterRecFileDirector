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
using RVMCore.EPGStationWarpper;
using RVMCore.EPGStationWarpper.Api;
using log4net;

namespace RVMCore
{
    public static class TVAFT
    {
        public static ILog LOG { get; private set; }

        public static bool IsNullOrEmptyOrWhiltSpace(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static bool SortFile(string[] margs)
        {
            LOG = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            LOG.Info("App started.");
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
                    LOG.Error(string.Format("Fail to read settings [{0}]",ex.Message));
                    Console.WriteLine("Sleep 10 sec...");
                    System.Threading.Thread.Sleep(10000);
                }
            }

            StreamFile mpars = null;
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
                LOG.Info(string.Format("App start with parameter\"{0}\"",sbb.ToString()));
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
                    LOG.Error("parameter -id not found or not a number.");
                    var e_id = Environment.GetEnvironmentVariable("RECORDEDID");
                    if (!e_id.IsNullOrEmptyOrWhiltSpace()&& !int.TryParse(e_id, out id))
                    {
                        Console.WriteLine("Error when dealing with [ID].", id.ToString());
                        LOG.Error("Failed to find \"RECORDEDID\" from Enviorment variable");                    
                        Console.WriteLine("Exiting...");
                        LOG.Info("App catch error. exiting...");
                        return false;
                    }
                    LOG.InfoFormat("Environment Variable \"RECORDEDID={0}\"", id.ToString());
                }
                Console.WriteLine("Preparing for access Epgstation server.");
                var mAccess = new EPGAccess(mySetting); 
                mpars = mAccess.GetStreamFileObj(id);
                if (mpars == null)
                {
                    Console.WriteLine("Remote file is missing or \"ID:{0}\" does not exsits.",id.ToString());
                    LOG.Info(string.Format("Remote file is missing or \"ID:{0}\" does not exsits.", id.ToString()));
                    Console.WriteLine("Exiting...");
                    LOG.Info("App catch error. exiting...");
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
            if (!MoveFile(mpars, mySetting))
            {
                Console.WriteLine("Local file is missing : \"File:{0}\" does not exsit.", System.IO.Path.GetFileName(mpars.FilePath));
                LOG.Error(string.Format("Local file is missing : \"File:{0}\" does not exsit.", System.IO.Path.GetFileName(mpars.FilePath)));
                LOG.Info("App catch error. exiting...");
                Console.WriteLine("Exiting...");
                return false;
            };
            LOG.Info("App has completed job. exiting...");
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

        private static bool MoveFile(StreamFile para,SettingObj mySetting)
        {
            if (!System.IO.File.Exists(para.FilePath)) return false;
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
                    {
                        System.IO.Directory.CreateDirectory(Targetfolder);
                        LOG.Info(string.Format("Create folder: {0}", Targetfolder));
                    }
                }
                FileMovier(para.FilePath, System.IO.Path.Combine(Targetfolder, fileName));
                para.FilePath = System.IO.Path.Combine(Targetfolder, fileName);
                para.ToXml(System.IO.Path.Combine(Targetfolder, fileName));
                OKBeep(mySetting);
            }
            else
            {
                FileMovier(para.FilePath, System.IO.Path.Combine(Targetfolder, fileName));
                para.FilePath = System.IO.Path.Combine(Targetfolder, fileName);
                para.ToXml(System.IO.Path.Combine(Targetfolder, fileName));
                OKBeep(mySetting);
            }
            return true;
        }

        private static bool FileMovier(string old , string tar)
        {
            bool check = false;
            do
            {
                System.IO.FileStream stream = null;
                try
                {
                    stream = System.IO.File.Open(old, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    check = true;
                }
                catch (System.IO.IOException)
                {
                    check = false;
                    System.Threading.Thread.Sleep(200);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            } while (!check);

            try
            {
                LOG.Info(string.Format("Moving file to: {0}", System.IO.Path.GetDirectoryName(tar)));
                System.IO.File.Move(old, tar);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! " + e.Message);
                LOG.Error(e.Message);
                return false;
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

    }
}
