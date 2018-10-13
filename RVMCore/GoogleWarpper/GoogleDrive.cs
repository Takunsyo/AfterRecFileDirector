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

namespace RVMCore.GoogleWarpper
{
    public delegate void UpdateProgress(int nowByte, int byteCount, int speed);

    public class GoogleDrive
    {
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
            req.Fields = "id, name, parents, mimeType";
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
        /// <para>Refence: https://developers.google.com/drive/api/v3/search-parameters
        /// </para></param>
        public IEnumerable<File> GetGoogleFiles(string querrystring = "")
        {
            string pageToken = null;
            List<File> tmp_list = new List<File>();
            do
            {
                var req = this.Service.Files.List();
                req.Spaces = "drive";
                req.Q = "'me' in owners" + (querrystring.IsNullOrEmptyOrWhiltSpace() ? "" : " and " + querrystring);
                req.Fields = "nextPageToken, files(id, name, parents, size, ownedByMe, md5Checksum)";
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
                tmp_list.AddRange(tmp.Files);
                pageToken = tmp.NextPageToken;
            } while (pageToken != null);
            return tmp_list;
        }

        /// <summary>
        /// Get a <see cref="IEnumerable{T}"/> of <see cref="File"/> from google by serach with 
        /// querryformat.
        /// </summary>
        /// <param name="querryformat">Serach string. 
        /// <para>Refence: https://developers.google.com/drive/api/v3/search-parameters
        /// </para></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        public IEnumerable<File> GetGoogleFiles(string querryformat, params object[] arg0)
        {
            return GetGoogleFiles(string.Format(querryformat, arg0));
        }

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
            return (mIDS == null) ? null : mIDS.Ids.First();
        }
        
        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public bool RemoteFileExists(string fullFilePath, IEnumerable<string> parentID)
        {
            if (parentID == null) parentID = new string[] { "root" };
            if (!System.IO.File.Exists(fullFilePath)) return false; 
            //getMD5Checksum
            string mMD5 = CalculateMD5(fullFilePath);
            string fileNameWithExtension = System.IO.Path.GetFileNameWithoutExtension(fullFilePath);
            var fList = this.GetGoogleFiles("name contains '{0}' and '{1}' in parents", fileNameWithExtension.CheckStringForQuerry(), parentID.First());
            if (fList == null) return false;
            foreach (File i in fList)
            {
                if(i.Md5Checksum == mMD5)return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public async Task<bool> RemoteFileExistsAsync(string fullFilePath, IEnumerable<string> parentID)
        {
            return await Task.Run(() => this.RemoteFileExists(fullFilePath, parentID));
        }

        /// <summary>
        /// Check if the file has exists at google drive.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="fullFilePath"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public bool RemoteFileExists(string fullFilePath, string remotePath = null)
        {
            if (remotePath!=null && !remotePath.IsNullOrEmptyOrWhiltSpace())
            {
                var parID = this.GetGoogleFolderByPath(remotePath);
                return this.RemoteFileExists(fullFilePath, new string[] { parID.Id });
            }
            else
            {
                return this.RemoteFileExists(fullFilePath, new string[] { "root" });
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
        public async Task<bool> RemoteFileExistsAsync(string fullFilePath, string remotePath = null)
        {
            return await Task.Run(() => this.RemoteFileExists(fullFilePath, remotePath));
        }




        /// <summary>
        /// Will be called when Upload or Download has a progress updated.
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

            using (System.IO.FileStream sr = new System.IO.FileStream(localPath,System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {                
                if (System.IO.File.Exists(GetUploadStatusPath(localPath)))
                {
                    //read local log for upload uri.
                    string rmuri = "";
                    using (System.IO.StreamReader uriRD = new System.IO.StreamReader(GetUploadStatusPath(localPath)))
                    {
                        rmuri = uriRD.ReadToEnd();
                    }
                    int chunkSize = 256*1024;
                    long byteCnt = -1;
                    Stopwatch mWatch ;
                    while (sr.Length!= sr.Position)
                    {//Start upload process
                        mWatch = new Stopwatch();
                        mWatch.Start();
                        var req = InitHttpWebRequest(rmuri);
                        req.Method = "PUT";
                        int counter = 0;
                        byte[] tmp = new byte[chunkSize]; ;
                        if (byteCnt < 0) //in case of .upload file exsits, this was first set to -1 means get upload progress from google server.
                        {//Get upload progress.
                            byteCnt = 0;
                            req.ContentLength = counter;
                            req.Headers.Add("Content-Range", string.Format("bytes */{0}", sr.Length));
                        }
                        else
                        {//Read chunk from local drive.
                            counter = sr.Read(tmp, 0, chunkSize);
                            if (counter <= 0) break;
                            req.ContentLength = counter;
                            req.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", byteCnt, byteCnt + counter - 1, sr.Length));
                        }                        
                        using (System.IO.Stream reqStream = req.GetRequestStream())
                        {                            
                            reqStream.Write(tmp, 0, counter);
                        }
                        
                        HttpWebResponse rep;
                        try //upload.
                        {
                            rep = (HttpWebResponse)req.GetResponse();
                        }
                        catch (WebException ex)
                        {
                            rep = (HttpWebResponse)ex.Response;
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
                                if (sr.Length > int.MaxValue)
                                {
                                    UpdateProgressChanged((int)(byteCnt / 2), (int)(sr.Length / 2), msp);
                                }
                                else
                                {
                                    UpdateProgressChanged((int)byteCnt, (int)sr.Length, msp);
                                }
                                break;
                            case 500:
                            case 502:
                            case 503:
                            case 504:
                                "Catch server error. Chunk[{1}/{2}] File:[{0}]".InfoLognConsole(localPath,byteCnt,sr.Length);
                                sr.Position -= counter;
                                //Server Errors retry current chunk
                                break;
                            case 403:
                                //Reached rate limit. suspend a few time then countinue.
                                "Reached rate limit. Suspend for [{1}]ms. File:[{0}]".InfoLognConsole(localPath,1000);
                                sr.Position -= counter;
                                Thread.Sleep(1000);
                                break;
                            case 404:
                                "Upload failed! Server has closed connection. File:[{0}]".ErrorLognConsole(localPath);
                                System.IO.File.Delete(GetUploadStatusPath(localPath));
                                "Retry upload.".InfoLognConsole();
                                this.UploadResumable(localPath, remotePath);
                                //Session has closed by server, restart upload from zero.
                                break;
                            case 308:
                                Console.WriteLine("Chunk[{0}/{1}] Success!", sr.Length);
                                string hd = rep.Headers.Get("Range");
                                if (hd.IsNullOrEmptyOrWhiltSpace())
                                {
                                    byteCnt = 0;
                                }
                                else { 
                                    hd = hd.Replace("bytes=", "");
                                    var itmp = hd.Split('-');
                                    if(itmp != null && itmp.Count() == 2)
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
                                System.IO.File.Delete(GetUploadStatusPath(localPath));
                                return null;
                                //Other message maybe Errors.
                        }

                        mWatch.Stop();
                        int speed = 0;
                        if (counter != 0)
                        {
                            speed = (int)(counter / mWatch.ElapsedMilliseconds * 1000);
                        }
                        if(sr.Length > int.MaxValue)
                        {
                            UpdateProgressChanged((int)(byteCnt/2), (int)(sr.Length/2), speed);
                        }
                        else
                        {
                            UpdateProgressChanged((int)byteCnt, (int)sr.Length, speed);
                        }
                    }
                }
                else
                {   //This is the first time tring to oupload this file.
                    //First create a file on google server.
                    string parentID = this.Root.Id;
                    if (!remotePath.IsNullOrEmptyOrWhiltSpace())
                    {
                        parentID = this.GetGoogleFolderByPath(remotePath).Id;
                    }
                    string mime = localPath.GetMimeType();
                    var insert = Service.Files.Create(new File() {
                        Name = System.IO.Path.GetFileName(localPath),
                        Parents = new string[] { parentID } }, sr, mime);
                    var uploadUri = insert.InitiateSessionAsync().Result;
                    var logPath = GetUploadStatusPath(localPath);
                    //Then leave a ".upload" file to record upload status.
                    using (System.IO.StreamWriter lsw = new System.IO.StreamWriter(logPath))
                    {
                        lsw.Write(uploadUri.ToString());
                    }
                    // Use resumable functions to upload file.
                    return this.UploadResumable(localPath, remotePath);
                }
            }
            FilesResource.ListRequest request = Service.Files.List();
            request.Q = string.Format("'{0}' in parents and name=''", GetGoogleFolderByPath(remotePath).Id, System.IO.Path.GetFileName(localPath));
            FileList result = request.Execute();
            if (result.Files.Count > 0)
                return result.Files[0];
            return null;
        }

        /// <summary>
        /// Awaitable Make a resumable upload request in the Google Drive API.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="localPath">Local file's full path.</param>
        /// <param name="remotePath">Google Drive Path : path looks like "\foo\bar\" to represent "GoogleDriveRoot:\foo\bar\".</param>
        /// <returns></returns>
        public async Task<File> UploadResumableAsync(string localPath, string remotePath)
        {
            return await Task.Run(() => UploadResumable(localPath, remotePath));
        }

        private HttpWebRequest InitHttpWebRequest(string uri)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Headers.Add("Authorization", "Bearer " + ((UserCredential)Service.HttpClientInitializer).Token.AccessToken);
            return req;
        }

        private string GetUploadStatusPath(string filePath)
        {
            return System.IO.Path.GetExtension(filePath).ToLower() ==".upload" ? 
                filePath : 
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath),System.IO.Path.GetFileNameWithoutExtension(filePath) + ".upload");
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
    }
}