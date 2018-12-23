using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net.Http.Headers;

namespace RVMCore.GoogleWarpper
{
    public delegate void UpdateProgress(long nowByte, long byteCount, int speed);
    
    public class GoogleDrive:IDisposable
    {
        /// <summary>
        /// The minimal byte count of google api.
        /// </summary>
        private const int BasePacketNumber = 256*1024;
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "After Record File Upload Service";
        static UserCredential UserCredential {
            get
            {
                return Credential.GetUserCredential();
            }
        }
        public DriveService Service { get; private set; }

        public Google.Apis.Drive.v3.Data.File Root { get; private set; }

        private ulong _MaxBytesPerSecond=0;

        /// <summary>
        /// Speed control, Acceptable minimum value is 256*1024. if disable speed control set value to 0.
        /// </summary>
        public ulong MaxBytesPerSecond {
            get
            {
                return _MaxBytesPerSecond;
            }
            set
            {
                if(value < BasePacketNumber && value !=0)
                {
                    this._MaxBytesPerSecond = BasePacketNumber;
                }
                else
                {
                    this._MaxBytesPerSecond = value;
                }
            }
        }

        private class Credential
        {
            [JsonProperty]
            private const string client_id = "397440156386-rs3uvlmkfakbkqfkc70lepv3ueem1ht3.apps.googleusercontent.com";
            [JsonProperty]
            private const string project_id = "mst-recordupload-1535044656173";
            [JsonProperty]
            private const string client_secret = "4vSX3zzWyv7HgY-PrORjG_5I";
            [JsonProperty]
            private const string auth_uri = "https://accounts.google.com/o/oauth2/auth";
            [JsonProperty]
            private const string token_uri = "https://www.googleapis.com/oauth2/v3/token";
            [JsonProperty]
            private const string auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
            [JsonProperty]
            private string[] redirect_uris = { "urn:ietf:wg:oauth:2.0:oob", "http://localhost" };

            static private string GetClientString()
            {
                return string.Format("{{\"installed\":{0}}}", JsonConvert.SerializeObject(new Credential()));
            }
            /// <summary>
            /// Get a new user credential object.
            /// </summary>
            /// <returns></returns>
            public static UserCredential GetUserCredential()
            {
                string credPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RVMGoogle", "4vSX3zzWyv7HgY-PrORjG_5I.json");
                using (var stream = new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(Credential.GetClientString())))
                {
                    return GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }

        }

        //private struct FolderStrucrt
        //{
        //    public string Name;
        //    public string ID;
        //    public File Body;
        //    public IEnumerable<FolderStrucrt> Childs;
        //}

        //private FolderStrucrt mRoot;
        /// <summary>
        /// Get a Path string point to a folder.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetRemotePathByID(string ID)
        {
            if (ID == Root.Id) return "";
            var mfile = GetGoogleFileByID(ID);
            var result = @"\" + mfile.Name;
            var mID = mfile.Parents.First();
            do
            {
                var mN = GetGoogleFileByID(mID);
                if(mN.Id == Root.Id)
                {
                    return result;
                }
                mID = mN.Parents.First();
                result = @"\" + mN.Name + result;
            } while (true);
        }

        /// <summary>
        /// Initialize a new instance of <see cref="GoogleDrive"/>.
        /// </summary>
        public GoogleDrive()
        {
            this.Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = UserCredential,
                ApplicationName = ApplicationName,
            });
            var m_root = Service.Files.Get("root");
            Root = m_root.Execute();
        }

        /// <summary>
        /// Get <see cref="File"/> object of a file identified by ID.
        /// </summary>
        /// <param name="fileID">folder id</param>
        /// <returns></returns>
        public File GetGoogleFileByID(string fileID)
        {
            var req = this.Service.Files.Get(fileID);
            req.Fields = "id, name, parents, mimeType, fileExtension, size, md5Checksum";
            try {
                return req.Execute();
            }
            catch (Exception ex) {
                ex.Message.ErrorLognConsole();
                "Cannot idenity file by ID({0})".ErrorLognConsole(fileID);
                return null;
            }
        }

        /// <summary>
        /// Get <see cref="File"/> object of a folder identified by ID.
        /// </summary>
        /// <param name="fileID">folder id</param>
        /// <returns></returns>
        public File GetGoogleFolderByID(string fileID)
        {
            var req = this.Service.Files.Get(fileID);
            req.Fields = "id, name, parents, mimeType";
            File tmp;
            try
            {
                tmp = req.Execute();
            }
            catch (Exception ex)
            {
                ex.Message.ErrorLognConsole();
                "Cannot idenitfy file by ID({0})".ErrorLognConsole(fileID);
                return null;
            }
            if (tmp.MimeType.Contains("application/vnd.google-apps.folder"))
            {
                return tmp;
            }
            else
            {
                "File \"{0}\"(ID={1}) is not a folder!".ErrorLognConsole(tmp.Name, tmp.Id);
                return null;
            }
        }

        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of <see cref="File"/> from google by serach with 
        /// querrystring.
        /// </summary>
        /// <param name="querrystring">Serach string. 
        /// <para>More information: <a cref="https://developers.google.com/drive/api/v3/search-parameters">Api refence</a>
        /// </para></param>
        public IEnumerable<File> GetGoogleFiles(string querrystring = "",bool OnlyOwnedByMe = true)
        {
            string pageToken = null;
            do
            {
                var req = this.Service.Files.List();
                req.Spaces = "drive";
                req.PageToken = pageToken;
                if (OnlyOwnedByMe)
                {
                    req.Q = "'me' in owners" + (querrystring.IsNullOrEmptyOrWhiltSpace() ? "" : $" and {querrystring}");
                }
                else
                {
                    req.Q = querrystring;
                }
                req.Fields = "nextPageToken, files(id, name, parents, size, ownedByMe, md5Checksum, trashed, mimeType)";
                FileList tmp;
                try
                {
                    tmp = req.Execute();
                }
                catch (Exception ex)
                {
                    ex.Message.ErrorLognConsole();
                    continue;
                }
                foreach(var i in tmp.Files)
                {
                    yield return i;
                }
                pageToken = tmp.NextPageToken;
            } while (pageToken != null);
        }

        /// <summary>
        /// Get a <see cref="IEnumerable{T}">IEnumerable&lt;File&gt;</see>of <see cref="File"/> from google by serach with 
        /// querrystring.
        /// </summary>
        /// <param name="querrystring">Serach string. 
        /// <para>More information: <a cref="https://developers.google.com/drive/api/v3/search-parameters">Api refence</a>
        /// </para></param>
        public IEnumerable<File> GetGoogleFiles(ref string nextPageTokenstring, string querrystring = "", bool OnlyOwnedByMe = true)
        {
            var req = this.Service.Files.List();
            req.Spaces = "drive";
            req.PageToken = nextPageTokenstring;
            if (OnlyOwnedByMe)
            {
                req.Q = "'me' in owners" + (querrystring.IsNullOrEmptyOrWhiltSpace() ? "" : " and " + querrystring);
            }
            else
            {
                req.Q = querrystring;
            }
            req.Fields = "nextPageToken, files(id, name, parents, size, ownedByMe, md5Checksum, trashed, mimeType)";
            FileList tmp;
            try
            {
                tmp = req.Execute();
            }
            catch (Exception ex)
            {
                ex.Message.ErrorLognConsole();
                return null;
            }
            nextPageTokenstring = tmp.NextPageToken;
            return tmp.Files;
        }

        /// <summary>
        /// Get a <see cref="IEnumerable{T}",T=File/> of <see cref="File"/> from google by serach with 
        /// querrystring.
        /// </summary>
        /// <param name="querrystring">Serach string. 
        /// <para>More information: <a cref="https://developers.google.com/drive/api/v3/search-parameters">Api refence</a>
        /// </para></param>
        public async Task<Tuple<IEnumerable<File>,string>> GetGoogleFilesAsync(string nextPageTokenstring, int maxContent = 100, string querrystring = "", bool OnlyOwnedByMe = true)
        {
            var req = this.Service.Files.List();
            req.Spaces = "drive";
            req.PageToken = nextPageTokenstring;
            if (OnlyOwnedByMe)
            {
                req.Q = "'me' in owners" + (querrystring.IsNullOrEmptyOrWhiltSpace() ? "" : " and " + querrystring);
            }
            else
            {
                req.Q = querrystring;
            }
            req.Fields = "nextPageToken, files(id, name, parents, size, ownedByMe, md5Checksum, trashed, mimeType)";
            FileList tmp;
            try
            {
                tmp = await req.ExecuteAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ErrorLognConsole();
                return null;
            }
            nextPageTokenstring = tmp.NextPageToken;
            return new Tuple<IEnumerable<File>, string>(tmp.Files,nextPageTokenstring);
        }
        
        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of <see cref="File"/> from google by serach with 
        /// querryformat.
        /// </summary>
        /// <param name="querryformat">Serach string. 
        /// <para>More information: <a cref="https://developers.google.com/drive/api/v3/search-parameters">Api refence</a></para></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public IEnumerable<File> GetGoogleFiles(string querryformat, params object[] arg0)=> GetGoogleFiles(string.Format(querryformat, arg0));
        

        /// <summary>
        /// Get a <see cref="File"/> object to represent the folder identified by a Path. 
        /// <para>Path must start at root folder.</para>
        /// </summary>
        /// <param name="path">A path looks like "\foo\bar\" to represent "GoogleDriveRoot:\foo\bar\".</param>
        /// <param name="create">If the folder in the path doesn't exists create as new folder or not.</param>
        /// <returns></returns>
        public File GetGoogleFolderByPath(string path, bool create = true)
        {
            string parentID = this.Root.Id;
            if (path.IsNullOrEmptyOrWhiltSpace()) return this.Root;
            var pList = path.Split('\\');
            if (pList == null) return this.Root;
            for (int i = 0; i < pList.Length; i++)
            {
                if (pList[i].IsNullOrEmptyOrWhiltSpace()) continue;
                var tmp = this.GetGoogleFiles("name='{0}' and '{1}' in parents", pList[i].Replace("\\","").Replace(@"'",@"\'"), parentID);
                if (tmp == null || tmp.Count() ==0)
                {
                    if (create)
                    {
                        var mFile = this.CreateGoogleFolder(new string[] { parentID }, pList[i]);
                        parentID = mFile.Id;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    parentID = tmp.First().Id;
                }
            }
            return this.GetGoogleFolderByID(parentID);
        }

        /// <summary>
        /// Create a <see cref="File"/> object and send to Google to make a new Folder.
        /// </summary>
        /// <param name="parent">A set of file IDs to identfy the path of the folder to make.</param>
        /// <param name="name">Folder name.</param>
        /// <returns></returns>
        public File CreateGoogleFolder(string[] parent, string name)
        {
            var body = new File();
            body.Name = name;
            body.Parents = parent;
            body.Id = this.GenerateID();
            body.MimeType = "application/vnd.google-apps.folder";
            var req = this.Service.Files.Create(body);
            req.Fields = "id, name, parents, mimeType";
            try
            {
                return req.Execute();
            }
            catch (Exception ex)
            {
                ex.Message.ErrorLognConsole();
                "Unable to create folder [{0}]".ErrorLognConsole(name);
                return null;
            }
        }

        /// <summary>
        /// Generates a file ID which can be provided in create requests.
        /// </summary>
        /// <returns>If success return a <see cref="string"/> as result. other will return <see cref="null"/>.</returns>
        public string GenerateID()
        {
            var tmp = this.Service.Files.GenerateIds();
            tmp.Space = "drive";
            tmp.Count = 1;
            GeneratedIds mIDS = null;
            try
            {
                mIDS = tmp.Execute();
            }
            catch (Exception ex)
            {
                ex.Message.ErrorLognConsole();
                "Unable to generate ID from google server.".ErrorLognConsole();
            }
            return mIDS?.Ids.First();
        }
        
        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public bool? RemoteFileExists(string fullFilePath, IEnumerable<string> parentID, bool checkMD5 = true)
        {
            if (parentID == null) parentID = new string[] { "root" };
            if (!System.IO.File.Exists(fullFilePath)) return false;
            //getMD5Checksum
            string mMD5 = null;
            using (var mTokenSource = new CancellationTokenSource())
            {
                Thread tgMD5 = new Thread(new ThreadStart(() => {
                    try { 
                        mMD5 = CalculateMD5(fullFilePath,mTokenSource.Token);
                        "MD5Cal Completed!".InfoLognConsole();
                    }
                    catch (OperationCanceledException)
                    {
                        "MD5Cal Canceled!".InfoLognConsole();
                    }
                    catch(Exception e)
                    {
                        "MD5Cal Error![{0}]".ErrorLognConsole(e.Message);
                    }
                }));
                if(checkMD5)tgMD5.Start();
                string fileNameWithExtension = System.IO.Path.GetFileName(fullFilePath);
                var fList = this.GetGoogleFiles("name contains '{0}' and '{1}' in parents", 
                                                fileNameWithExtension.CheckStringForQuerry(), 
                                                parentID.First());
                if ((fList?.Count() ?? 0) == 0)
                {
                    if (tgMD5.IsAlive) mTokenSource.Cancel();
                    return false;
                }
                if (fList.Count() > 1 && !checkMD5) return null;
                if(fList.Count() > 1)
                {
                    if (tgMD5.IsAlive) tgMD5.Join();
                    foreach (var i in fList)
                    {
                        if (i.Md5Checksum == mMD5) return true;
                    }
                }
                if (!checkMD5) return true;
                if (tgMD5.IsAlive) tgMD5.Join();
                return fList.Any(x => x.Md5Checksum == mMD5);
            }
        }

        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public async Task<bool?> RemoteFileExistsAsync(string fullFilePath, IEnumerable<string> parentID,bool checkMD5 = true) =>
            await Task.Run(() => this.RemoteFileExists(fullFilePath, parentID,checkMD5));
        
        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public bool? RemoteFileExists(string fullFilePath, string remotePath = null,bool checkMD5 = true)
        {
            if (!(remotePath?.IsNullOrEmptyOrWhiltSpace() ?? true))
            {
                var parID = this.GetGoogleFolderByPath(remotePath);
                return this.RemoteFileExists(fullFilePath, new string[] { parID.Id },checkMD5);
            }
            else
            {
                return this.RemoteFileExists(fullFilePath, new string[] { "root" },checkMD5);
            }
        }

        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async Task<bool?> RemoteFileExistsAsync(string fullFilePath, string remotePath = null) => 
            await Task.Run(() => this.RemoteFileExists(fullFilePath, remotePath));
        
        /// <summary>
        /// This event will be raised when Upload or Download has a progress updated.
        /// </summary>
        public event UpdateProgress UpdateProgressChanged;

        /// <summary>
        /// Make a resumable upload request in the Google Drive API.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="localPath">Local file's full path.</param>
        /// <param name="remotePath">Google Drive Path : path looks like "\foo\bar\" to represent "GoogleDriveRoot:\foo\bar\".</param>
        /// <returns></returns>
        public File UploadResumable(string localPath, string remotePath)
        {
            if (!System.IO.File.Exists(localPath))
            {
                "Local file [{0}] is missing!!".ErrorLognConsole(localPath);
                return null;
            }
            System.IO.FileStream sr = new System.IO.FileStream(localPath,System.IO.FileMode.Open, System.IO.FileAccess.Read);
            if (System.IO.File.Exists(GetUploadStatusPath(localPath)))
            {
                //read local log for upload uri.
                string rmuri = "";
                using (System.IO.StreamReader uriRD = new System.IO.StreamReader(GetUploadStatusPath(localPath)))
                {
                    rmuri = uriRD.ReadToEnd();
                }
                //Console.WriteLine("Upload meta Oped.");
                int chunkSize = BasePacketNumber;
                long byteCnt = -1;
                Stopwatch mWatch = new Stopwatch(); ;
                Stopwatch SpeedControl = null;
                List<int> evaSpeed = new List<int>();
                long spdCnt = 0;
                while (sr.Length != sr.Position)
                {//Start upload process
                    if (this.MaxBytesPerSecond != 0)
                    {
                        if(!(SpeedControl?.IsRunning ?? false))
                        { 
                            SpeedControl = new Stopwatch();
                            SpeedControl.Start();
                        }
                    }
                    else
                    {
                        SpeedControl?.Stop();
                        SpeedControl = null;
                    }
                    mWatch.Restart();
                    int counter = 0;
                    byte[] tmp = new byte[chunkSize]; ;
                    HttpWebRequest req =null;
                    if (!(byteCnt < 0)) //in case of .upload file exsits, this was first set to -1 means get upload progress from google server.
                    //Get upload progress.
                    {//Read chunk from local drive.
                        counter = sr.Read(tmp, 0, chunkSize);
                        if (counter <= 0) break;
                    }
                    bool isSend = false;
                    ThreadStart WorkLoad = new ThreadStart(() => {
                        req = InitHttpWebRequest(rmuri);
                        req.Method = "PUT";
                        req.Timeout = (5000);
                        if (byteCnt < 0)
                        {//in case of .upload file exsits, this was first set to -1 means get upload progress from google server.
                            //Get upload progress.
                            byteCnt = 0;
                            req.ContentLength = counter;
                            req.Headers.Add("Content-Range", string.Format("bytes */{0}", sr.Length));
                        }
                        else
                        {
                            req.ContentLength = counter;
                            req.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", byteCnt, byteCnt + counter - 1, sr.Length));
                        }
                        try
                        {
                            using (System.IO.Stream reqStream = req.GetRequestStream())
                            {
                                reqStream.Write(tmp, 0, counter);
                                isSend = true;
                            }
                        }
                        catch (WebException ex)
                        {
                            req.Abort();
                            req.ServicePoint.CloseConnectionGroup(req.ConnectionGroupName);
                            ex.Message.ErrorLognConsole();
                            "Socket timeout or unknown local Errors [File:{0}]".InfoLognConsole(localPath);
                            if (byteCnt == 0) byteCnt -= 1;
                            isSend = false;
                        }
                    });
                    do
                    {
                        Stopwatch counterTimeout = new Stopwatch();
                        counterTimeout.Start();
                        var mWork = new Thread(WorkLoad);
                        mWork.Start();
                        do
                        {
                            if (counterTimeout.ElapsedMilliseconds > 5000 && mWork.IsAlive)
                            {
                                req.Abort();
                                "Timeout! reset thread".ErrorLognConsole();
                                if (byteCnt == 0) byteCnt -= 1;
                                mWork.Abort();
                                break;
                            }
                            Thread.Sleep(200);
                        } while (!isSend);
                        if (!isSend)
                        {
                            Thread.Sleep(1000);
                            GC.Collect();
                        }
                    } while (!isSend);

                    HttpWebResponse rep;
                    try //upload.
                    {
                        Console.WriteLine("Try getting response.");
                        rep = (HttpWebResponse)req.GetResponse();
                    }
                    catch (WebException ex)
                    {
                        rep = (HttpWebResponse)ex.Response;
                    }
                    req.Abort();//This is important here. If don't the upload procress will stop by HTT
                    if (rep == null)
                    {
                        "Local Error! [File:{0}]".InfoLognConsole(localPath);
                        sr.Position -= counter;
                        continue;
                    }
                    //Handle server responses.
                    switch ((int)rep.StatusCode)
                    {
                        case 200:
                        case 201:
                            // OK
                            "Upload complete! File:[{0}]".InfoLognConsole(localPath);
                            System.IO.File.Delete(GetUploadStatusPath(localPath));
                            int msp = 0;
                            if (counter != 0)
                            {
                                msp = (int)(counter / mWatch.ElapsedMilliseconds * 1000);
                            }
                            evaSpeed.Add(msp);
                            if (evaSpeed.Count > 10) evaSpeed.RemoveAt(0);
                            UpdateProgressChanged?.Invoke(sr.Length, sr.Length, (int)evaSpeed.Average());
                            break;
                        case 500:
                        case 502:
                        case 503:
                        case 504:
                            "Catch server error. Chunk[{1}/{2}] File:[{0}]".InfoLognConsole(localPath, byteCnt, sr.Length);
                            sr.Position -= counter;
                            //Server Errors retry current chunk
                            break;
                        case 403:
                            //Reached rate limit. suspend a few time then countinue.
                            "Reached rate limit. Suspend for [{1}]ms. File:[{0}]".InfoLognConsole(localPath, 1000);
                            sr.Position -= counter;
                            Thread.Sleep(1000);
                            break;
                        case 404:
                            "Upload failed! Server has closed connection. File:[{0}]".ErrorLognConsole(localPath);
                            if (!DeleteTempFile(GetUploadStatusPath(localPath))) return null; ;
                            "Retry upload.".InfoLognConsole();
                            sr.Dispose();//Close file stream, if not thread will hang forever try to open file.
                            return this.UploadResumable(localPath, remotePath);
                            //Session has closed by server, restart upload from zero.
                        case 308:
                            Console.WriteLine("Chunk[{0}/{1}] Success!", byteCnt, sr.Length);
                            string hd = rep.Headers.Get("Range");
                            if (hd.IsNullOrEmptyOrWhiltSpace())
                            {
                                byteCnt = 0;
                            }
                            else
                            {
                                hd = hd.Replace("bytes=", "");
                                var itmp = hd.Split('-');
                                if (itmp != null && itmp.Count() == 2)
                                {
                                    long.TryParse(itmp[1], out byteCnt);
                                    byteCnt += 1;
                                    sr.Position = byteCnt;
                                }
                            }
                            //Need to countinue upload.
                            break;
                        default:
                            using (System.IO.Stream tmpst = rep.GetResponseStream())
                            {
                                var mmm = new byte[int.Parse(rep.Headers.Get("Content-Length"))];
                                tmpst.ReadAsync(mmm, 0, mmm.Length);
                                Encoding.UTF8.GetString(mmm).ErrorLognConsole();
                            }
                            "Upload failed! Catch unhandled error! File:[{0}]".ErrorLognConsole(localPath);
                            DeleteTempFile(GetUploadStatusPath(localPath));
                            sr.Dispose();
                            return null;
                            //Other message maybe Errors.
                    }
                    rep.Close();
                    if (this.MaxBytesPerSecond != 0 && (SpeedControl?.IsRunning ?? false)) { 
                        spdCnt += counter;
                        if (spdCnt >= (long)this.MaxBytesPerSecond)
                        {
                            if (SpeedControl.ElapsedMilliseconds < 1000)
                            {
                                Thread.Sleep((int)(1000 - SpeedControl.ElapsedMilliseconds));
                            }
                            spdCnt = 0;
                            SpeedControl.Restart();
                        }
                    }
                    mWatch.Stop();
                    int speed = 0;
                    if (counter != 0)
                    {
                        speed = (int)(counter / mWatch.ElapsedMilliseconds * 1000);
                        var packetMilisec = mWatch.ElapsedMilliseconds;
                        if(MaxBytesPerSecond != 0)
                        {
                            if(speed > (int)MaxBytesPerSecond)
                                chunkSize = (int)MaxBytesPerSecond;
                            else
                                chunkSize = BasePacketNumber * (int)(speed / BasePacketNumber);
                        }
                        else
                            chunkSize = BasePacketNumber*(int)(speed/BasePacketNumber);
                        if(chunkSize < BasePacketNumber)
                            chunkSize = BasePacketNumber;
                    }
                    evaSpeed.Add(speed);
                    if (evaSpeed.Count > 10) evaSpeed.RemoveAt(0);
                    UpdateProgressChanged?.Invoke(byteCnt, sr.Length, (int)evaSpeed.Average());
                }
                SpeedControl?.Stop();
            }
            else
            {   //This is the first time tring to oupload this file.
                //First create a file on google server.
                string parentID = this.Root.Id;
                if (!remotePath.IsNullOrEmptyOrWhiltSpace())
                {
                    parentID = this.GetGoogleFolderByPath(remotePath).Id;
                }
                string mime = localPath.GetMimeType();//application/octet-stream
                var insert = Service.Files.Create(new File() {
                    Name = System.IO.Path.GetFileName(localPath),
                    Parents = new string[] { parentID } ,
                    MimeType = mime}, sr, mime);
                var uploadUri = insert.InitiateSessionAsync().Result;
                var logPath = GetUploadStatusPath(localPath);
                //Then leave a ".upload" file to record upload status.
                using (System.IO.StreamWriter lsw = new System.IO.StreamWriter(logPath))
                {
                    lsw.Write(uploadUri.ToString());
                }
                // Use resumable functions to upload file.
                sr.Dispose();//Close stream,if not it will wait forever to open the same file.
                return this.UploadResumable(localPath, remotePath);
            }
            sr.Dispose();
            FilesResource.ListRequest request = Service.Files.List();
            request.Q = string.Format("'{0}' in parents and name contains '{1}'", GetGoogleFolderByPath(remotePath).Id, System.IO.Path.GetFileName(localPath).CheckStringForQuerry());
            FileList result = request.Execute();
            if (result.Files.Count > 0)
                return result.Files[0];
            return null;
            bool DeleteTempFile(string filePath)
            {
                try
                {
                    System.IO.File.SetAttributes(filePath, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(filePath);
                }
                catch
                {
                    "Unable to delete temp file!".ErrorLognConsole();
                    return false;
                }
                return true;    
            }
        }

        /// <summary>
        /// Awaitable Make a resumable upload request in the Google Drive API.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="localPath">Local file's full path.</param>
        /// <param name="remotePath">Google Drive Path : path looks like "\foo\bar\" to represent "GoogleDriveRoot:\foo\bar\".</param>
        /// <returns></returns>
        public async Task<File> UploadResumableAsync(string localPath, string remotePath)=>        
            await Task.Run(() => UploadResumable(localPath, remotePath));
       
        /// <summary>
        /// Update certain files metadata.
        /// </summary>
        /// <exception cref="WebException"></exception>
        /// <param name="ID">GoogleDrive's file ID</param>
        /// <param name="body">A <see cref="File"/> object of the file
        /// <para>Only a part of the <see cref="File">'s property can be used.</para>
        /// </param>
        public File UpdateFile(string ID, in File body)
        {
            var req = Service.Files.Update(body, ID);
            return req.Execute();
        }

        /// <summary>
        /// Make a resumable Download request to the GoogleDrive.
        /// </summary>
        /// <param name="ID">Google drive File ID</param>
        /// <param name="localPath">Local folder or a specified file's path.</param>
        /// <param name="checkMD5">Assign to <see cref="true"/> if you want to check the file with MD5 sum check.</param>
        /// <returns><see cref="true"/> if the file has successfully downloaded.</returns>
        public bool DownloadResumable(string ID, string localPath, bool checkMD5)
        {
            //Get file meta first.
            var mFile = this.GetGoogleFileByID(ID);
            if (mFile == null) return false;
            //Ready local file.
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(localPath);
            string tmpPath = null;
            string tarPath = null;
            if ((attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
            {//if input path is a folder.
                if (System.IO.File.Exists(System.IO.Path.Combine(localPath, mFile.Name)))
                {
                    "Upload Canceld :There is a file has the same name with [{0}]".ErrorLognConsole(mFile.Name);
                    return false;
                }
                tmpPath = GetDownloadStatusPath(System.IO.Path.Combine(localPath, mFile.Name));
                tarPath = System.IO.Path.ChangeExtension(tmpPath, mFile.FileExtension);
            }
            else
            {
                tmpPath = GetDownloadStatusPath(localPath);
                tarPath = System.IO.Path.ChangeExtension(tmpPath, mFile.FileExtension);
                if (System.IO.File.Exists(tarPath))
                {
                    "Upload Canceld :There is a file has the same name with [{0}]".ErrorLognConsole(mFile.Name);
                    return false;
                }
            }
            //Make request object.
            //var req = Service.Files.Get(ID);
            //Ready others
            int chunkSize = 1024 * 1024;
            Stopwatch mWatch = new Stopwatch(); //packet speed counter.
            Stopwatch SpeedControl = null;      //speed control counter.
            long spdCnt = 0;
            List<int> evaSpeed = new List<int>();
            //Open file to write.
            System.IO.FileStream sw;
            try
            {
                sw = new System.IO.FileStream(tmpPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                ex.Message.ErrorLognConsole();
                return false;
            }
            //MD5 SumCheck objects:
            var md5 = MD5.Create();
            if (sw.Length > 0 & checkMD5)
            {
                do{
                    byte[] buffer = new byte[chunkSize];
                    if ((sw.Position + chunkSize) > sw.Length)
                    {
                        int mLen = (int)(sw.Length - sw.Position);
                        buffer = new byte[mLen];
                        sw.Read(buffer, 0, mLen);
                    }
                    else
                    {
                        sw.Read(buffer, 0, chunkSize);
                    }
                    md5.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
                    UpdateProgressChanged?.Invoke(sw.Position, (long)mFile.Size, 0);
                    if (sw.Position == sw.Length) break;
                } while (true);
            }
            sw.Position = sw.Length;
            do
            {
                //Speed control.
                if (this.MaxBytesPerSecond != 0)
                {
                    if (SpeedControl == null || !SpeedControl.IsRunning)
                    {
                        SpeedControl = new Stopwatch();
                        SpeedControl.Start();
                    }
                }
                else
                {
                    if(SpeedControl !=null)SpeedControl.Stop();
                    SpeedControl = null;
                }
                mWatch.Restart();
                int mChunk = chunkSize;
                if ((sw.Position + chunkSize) > mFile.Size)
                {
                    mChunk = (int)(mFile.Size - sw.Position);
                }
                var dwnUri = @"https://www.googleapis.com/drive/v3/files/" + ID + "?alt=media";
                HttpWebRequest req; HttpWebResponse rep =null;
                while (true)
                {
                    req = InitHttpWebRequest(dwnUri);
                    req.Method = "GET";
                    req.AddRange(sw.Position, sw.Position + mChunk);
                    try
                    {
                        rep = (HttpWebResponse)req.GetResponseAsync().Result;
                    }
                    catch (WebException e)
                    {
                        e.Message.ErrorLognConsole();
                    }
                    if (rep != null) break;
                    else req.Abort();
                    GC.Collect();
                }
                //RangeHeaderValue mRange = new RangeHeaderValue(sw.Position, sw.Position + mChunk);
                //req.Headers.Add("Range:"+mRange.ToString());
                byte[] buffer = new byte[mChunk];
                using (var ms = new System.IO.MemoryStream())
                {
                    try
                    {
                        using (var msult = rep.GetResponseStream())
                        {
                            int rc = msult.ReadAsync(buffer, 0, mChunk).Result;
                            if (rc <= 0) Debugger.Break();
                            msult.Flush();
                        }
                            //req.DownloadRange(ms, mRange);
                    }
                    catch(Exception ex)
                    {
                        ex.Message.ErrorLognConsole();
                        continue;
                    }
                    //buffer = ms.ToArray();
                }
                sw.Write(buffer, 0, mChunk);
                Console.WriteLine("Chunk[{0},{1}] downloaded", sw.Position, mFile.Size);
                if (checkMD5) {
                    if (mChunk != chunkSize)
                    {
                        md5.TransformFinalBlock(buffer, 0, buffer.Length);
                    } // if chunkSize has changed then break the loop.
                    else
                    {
                        md5.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
                    }
                }
                if (this.MaxBytesPerSecond != 0 && SpeedControl != null && SpeedControl.IsRunning)
                {
                    spdCnt += mChunk;
                    if (spdCnt >= (long)this.MaxBytesPerSecond)
                    {
                        if (SpeedControl.ElapsedMilliseconds < 1000)
                        {
                            Thread.Sleep((int)(1000 - SpeedControl.ElapsedMilliseconds));
                        }
                        spdCnt = 0;
                        SpeedControl.Restart();
                    }
                }
                req.Abort();
                mWatch.Stop();
                int speed = 0;
                if (mChunk != 0)
                {
                    speed = (int)(mChunk / mWatch.ElapsedMilliseconds * 1000);
                }
                evaSpeed.Add(speed);
                if (evaSpeed.Count > 10) evaSpeed.RemoveAt(0);
                UpdateProgressChanged?.Invoke(sw.Position, (long)mFile.Size, (int)evaSpeed.Average());
                if (mChunk != chunkSize) break;
            } while (true);
            sw.Close();
            //Rename file to orignal.
            try
            {
                System.IO.File.Move(tmpPath, tarPath);
            }
            catch(Exception ex)
            {
                "Failed to rename file!".ErrorLognConsole();
                ex.Message.ErrorLognConsole();
                return false;
            }
            if (BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant() != mFile.Md5Checksum)
            {
                return false;
            }
            "Download completed! File:{0}]".InfoLognConsole(tarPath);
            return true;
        }

        /// <summary>
        /// Make a resumable Download request to the GoogleDrive.
        /// </summary>
        /// <param name="ID">Google drive File ID</param>
        /// <param name="localPath">Local folder or a specified file's path.</param>
        /// <returns><see cref="true"/> if the file has successfully downloaded.</returns>
        public bool DownloadResumable(string ID, string localPath)=>
            DownloadResumable(ID, localPath, false);

        /// <summary>
        /// Make a awaitable resumable Download request to the GoogleDrive.
        /// </summary>
        /// <param name="ID">Google drive File ID</param>
        /// <param name="localPath">Local folder or a specified file's path.</param>
        /// <param name="checkMD5">Assign to <see cref="true"/> if you want to check the file with MD5 sum check.</param>
        /// <returns><see cref="true"/> if the file has successfully downloaded.</returns>
        public async Task<bool> DownloadResumableAsync(string ID, string localPath, bool checkMD5)=> 
            await Task.Run(() => DownloadResumable(ID, localPath, checkMD5));
        
        /// <summary>
        /// Make a awaitable resumable Download request to the GoogleDrive.
        /// </summary>
        /// <param name="ID">Google drive File ID</param>
        /// <param name="localPath">Local folder or a specified file's path.</param>
        /// <returns><see cref="true"/> if the file has successfully downloaded.</returns>
        public async Task<bool> DownloadResumableAsync(string ID, string localPath) =>
            await Task.Run(() => DownloadResumable(ID, localPath, false));
        
        /// <summary>
        /// To replace a File with a updated MimeType. *Proved not working.
        /// </summary>
        /// <param name="ID">Google Drive File's ID</param>
        /// <param name="NewMimeType">New MimeType</param>
        /// <param name="DeleteOrignal">Set to false by default, Set to true if delete permanently.
        /// <para>*Notice: if set to <see cref="true"/>, the deletion can *NOT be undo.</para></param>
        /// <returns></returns>
        [Obsolete("This method is not working.",true)]
        public bool ReplaceFileMimetype(string ID,string NewMimeType,bool DeleteOrignal = false)
        {
            var oriFile = this.GetGoogleFileByID(ID);
            if (oriFile == null) return false;
            var newFile = new File();
            newFile.Name = oriFile.Name;
            newFile.Parents = oriFile.Parents;
            newFile.MimeType = oriFile.MimeType;//NewMimeType;
            //string jsonBody = "{\"mimeType\":\"" + NewMimeType +
            //                    "\",\"name\":\"" + oriFile.Name + "\","+ "\"parents\":[";
            //foreach( string i in oriFile.Parents)
            //{
            //    jsonBody = jsonBody + "\"" + i + "\",";
            //}
            //jsonBody = jsonBody.Remove(jsonBody.Length - 1);
            //jsonBody = jsonBody + "]}";
            //string uri = @"https://www.googleapis.com/drive/v3/files/"+ ID +@"/copy";
            //var req = this.InitHttpWebRequest(uri);
            //req.Method = "POST";
            //var buffer = Encoding.UTF8.GetBytes(jsonBody);
            //using (var st = req.GetRequestStream())
            //{
            //    st.Write(buffer, 0, buffer.Length);
            //}
            //using(var rep = req.GetResponse())
            //{
            //    if (rep ==null) return false;
            //}
            newFile = this.Service.Files.Copy(newFile, ID).Execute();
            if (newFile == null) return false;
            DeleteFile(ID, !DeleteOrignal);
            return true;
        }

        /// <summary>
        /// Delete a file permanently or put a file in to trash.
        /// <para>Trash function is using Google Drive API V2 direct http post request.</para>
        /// </summary>
        /// <param name="ID">The file's ID.</param>
        /// <param name="PutInTrash">Set to <see cref="true"/> by Default, If delete file permanently set it to <see cref="false"/>. 
        /// <para>*Notice: if set to <see cref="false"/>, this action can *NOT be undo.</para></param>
        /// <returns></returns>
        public bool DeleteFile(string ID, bool PutInTrash = true)
        {
            if (PutInTrash)
            {
                var uri = String.Format("https://www.googleapis.com/drive/v2/files/{0}/trash", ID);
                var req = this.InitHttpWebRequest(uri);
                req.Method = "POST";
                req.ContentLength = 0;
                try
                {
                    req.GetResponse();
                }
                catch(WebException ex)
                {
                    ex.Message.ErrorLognConsole();
                    return false;
                }
                return true;
            }
            else
            {
                var req = this.Service.Files.Delete(ID);
                try
                {
                    req.Execute();
                }
                catch(Exception ex)
                {
                    ex.Message.ErrorLognConsole();
                    return false;
                }
                return true;
            }
        }

        private HttpWebRequest InitHttpWebRequest(string uri)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Headers.Add("Authorization", "Bearer " + ((UserCredential)Service.HttpClientInitializer).Token.AccessToken);
            req.KeepAlive = false;
            req.AllowWriteStreamBuffering = false;
            req.ProtocolVersion = HttpVersion.Version11;
            req.ReadWriteTimeout = 5000;
            req.UserAgent = "RVMCore.GoogleClient";
            return req;
        }

        public static string GetUploadStatusPath(string filePath)
        {
            return System.IO.Path.GetExtension(filePath).ToLower() ==".upload" ? 
                filePath : 
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath),System.IO.Path.GetFileNameWithoutExtension(filePath) + ".upload");
        }

        public static string GetDownloadStatusPath(string filePath)
        {
            return System.IO.Path.GetExtension(filePath).ToLower() == ".part" ?
                filePath :
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), System.IO.Path.GetFileNameWithoutExtension(filePath) + ".part");
        }

        private static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Calculate MD5 hash for file, can be stop.         
        /// <para>
        /// See how to use : 
        /// <a cref="https://stackoverflow.com/questions/36705792/stop-hashing-operation-using-filestream">
        /// Link to Starkoverflow</a></para>
        /// </summary>
        /// <example>
        /// <code>
        /// try {
        ///     var md5 = <c>CalculateMD5</c>(path,ct)
        /// }
        /// catch(OperationCanceledException){
        ///     ActionsWhenCanceled.
        /// }
        /// </code></example>
        /// <param name="path">Full local file path.</param>
        /// <param name="ct">Stopper <see cref="CancellationToken"/></param>
        /// <returns>A Hash MD5 String.</returns>
        static string CalculateMD5(string path, CancellationToken ct)
        {
            using (var stream = System.IO.File.OpenRead(path))
            {
                using (var md5 = MD5.Create())
                {
                    const int blockSize = 1024 * 1024 * 4;
                    var buffer = new byte[blockSize];
                    long offset = 0;

                    while (true)
                    {
                        ct.ThrowIfCancellationRequested();
                        var read = stream.Read(buffer, 0, blockSize);
                        if (stream.Position == stream.Length)
                        {
                            md5.TransformFinalBlock(buffer, 0, read);
                            break;
                        }
                        offset += md5.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
                        //Console.WriteLine($"Processed {offset * 1.0 / 1024 / 1024} MB so far");
                    }
                    return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public void Dispose()
        {
            ((IDisposable)Service).Dispose();
            Root = null;
            GC.Collect();
        }
    }
}