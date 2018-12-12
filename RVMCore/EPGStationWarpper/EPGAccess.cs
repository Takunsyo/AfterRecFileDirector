using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RVMCore.EPGStationWarpper.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RVMCore.EPGStationWarpper
{
    /// <summary>
    /// Object for accessing EPGStation.
    /// </summary>
    public class EPGAccess
    {
        public NetworkCredential EPGCredential { get; }
        public string ServiceAddr { get; }
        public string BaseFolder { get; }

        public EPGAccess(SettingObj setting)
        {
            EPGCredential = new NetworkCredential(setting.EPG_UserName, setting.EPG_Passwd);
            ServiceAddr = setting.EPG_ServiceAddr;
            BaseFolder = System.IO.Path.Combine(setting.StorageFolder, setting.EPG_BaseFolder);
        }

        public EPGAccess(string uname, string passwd, string servAddr, string bFolder)
        {
            EPGCredential = new NetworkCredential(uname, passwd);
            ServiceAddr = servAddr;
            BaseFolder = bFolder;
        }

        #region private methods 

        private enum RequestMethods
        {
            POST,
            GET,
            PUT,
            DELETE
        }

        private HttpWebRequest InitWebRequest(Uri uri, RequestMethods method = RequestMethods.GET)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = "RVMCore.EPGAccess/" + Environment.Version;
            req.ContentType = "application/json; charset=utf-8";
            switch (method)
            {
                case RequestMethods.GET:
                    req.Method = "GET";
                    break;
                case RequestMethods.POST:
                    req.Method = "POST";
                    break;
                case RequestMethods.PUT:
                    req.Method = "PUT";
                    break;
                case RequestMethods.DELETE:
                    req.Method = "DELETE";
                    break;
                default:
                    req.Method = "GET";
                    break;
            }
            req.Credentials = this.EPGCredential;
            return req;
        }

        private HttpWebRequest InitWebRequest(string uri, RequestMethods method = RequestMethods.GET)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = "RVMCore.EPGAccess/" + Environment.Version;
            req.ContentType = "application/json; charset=utf-8";
            switch (method)
            {
                case RequestMethods.GET:
                    req.Method = "GET";
                    break;
                case RequestMethods.POST:
                    req.Method = "POST";
                    break;
                case RequestMethods.PUT:
                    req.Method = "PUT";
                    break;
                case RequestMethods.DELETE:
                    req.Method = "DELETE";
                    break;
                default:
                    req.Method = "GET";
                    break;
            }
            req.Credentials = this.EPGCredential;
            return req;
        }

        private HttpWebResponse GettingResponse(ref HttpWebRequest request)=> GettingResponse(ref request, out _);

        private HttpWebResponse GettingResponse(ref HttpWebRequest request, out Exception e)
        {
            try
            {
                e = null;
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                e = null;
                return (HttpWebResponse)ex.Response;
            }
            catch (Exception ex)
            {
                e = ex;
                return null;
            }
        }

        private HttpWebResponse GettingResponse(ref HttpWebRequest request,byte[] data)=> GettingResponse(ref request, data, out _);

        private HttpWebResponse GettingResponse(ref HttpWebRequest request,byte[] data, out Exception e)
        {
            try
            {
                e = null;
                using (var send = request.GetRequestStream())
                {
                    send.Write(data, 0, data.Count());
                }
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                e = null;
                return (HttpWebResponse)ex.Response;
            }
            catch (Exception ex)
            {
                e = ex;
                return null;
            }
        }

        private string GetResponseBody(ref HttpWebResponse response, Encoding encoding, bool CloseAfterWork = true)
        {
            if (response == null || response.ContentLength <= 0) return null;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                var tmp = reader.ReadToEnd();
                if (CloseAfterWork) response.Close();
                return tmp;
            }
        }

        private byte[] GetResponseBody(ref HttpWebResponse response, bool CloseAfterWork = true)
        {
            if (response == null || response.ContentLength <= 0) return null;
            var length = response.ContentLength;
            using (var reader = response.GetResponseStream())
            {
                var buffer = new byte[length];
                reader.Read(buffer, 0, (int)length);
                if (CloseAfterWork) response.Close();
                return buffer;
            }
        }
        #endregion

        #region "API Recored"
        /// <summary>
        /// Get Recorded program by record ID.
        /// <para>EPGStation api "/recorded/{id}"</para>
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns></returns>
        public Program GetRecordProgramByID(int id)
        {
            if (id <= 0) return null;
            string EPGATTR = "http://{1}/api/recorded/{0}";
            var ub = new UriBuilder(string.Format(EPGATTR, id.ToString(), this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            //if (networkID != null) query["networkId"] = networkID.ToString();
            //if (serviceID != null) query["serviceId"] = serviceID.ToString();
            //if (eventID != null) query["eventId"] = eventID.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<Program>(body);
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get program! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();

                    return null;
            }
        }

        /// <summary>
        /// Get <see cref="StreamFile"/> object from EPGStation's recorded file by id.
        /// </summary>
        public StreamFile GetStreamFileObj(int id)
        {
            //return this.GetRecordProgramByID(id).GetStreamFileObj(this);
            StreamFile mFile = new StreamFile();
            var body = this.GetRecordProgramByID(id);
            if (body == null) return null;
            mFile.ChannelName = this.GetChannelNameByID(body.channelId);
            mFile.Content = body.description;
            mFile.Infomation = body.extended;
            mFile.Title = body.name;
            mFile.recTitle = body.name;
            mFile.recKeyWord = "EPGStation " + body.id.ToString();
            mFile.recSubTitle = body.videoResolution + body.videoResolution + body.videoType;
            mFile.recKeywordInfo = string.Format("Error:E({0})D({1})S({2})", body.errorCnt, body.dropCnt, body.scramblingCnt);
            mFile.Genre = body.Genre;
            mFile.StartTime = Helper.GetTimeFromUNIXTime(body.startAt);
            mFile.EndTime = Helper.GetTimeFromUNIXTime(body.endAt);
            mFile.FilePath = System.IO.Path.Combine(this.BaseFolder, System.Web.HttpUtility.UrlDecode(body.filename));
            mFile.EPGStation = new EPGMetaFile(this, body);
            return mFile;
        }

        /// <summary>
        /// Get all recorded files. notice paramter <paramref name="limit"/> will determen the max count of recods by action.
        /// <para>EPGStation api "/recorded"</para>
        /// </summary>
        /// <param name="rule_id">Record rule's id</param>
        /// <param name="genre">Program genre's EPGStation enum id</param>
        /// <param name="channel_id">Channel id</param>
        /// <param name="keyword">Keyword to find a program.</param>
        /// <param name="limit">max count of recods by action.</param>
        /// <param name="offset">the offset to the list on the server</param>
        /// <param name="reverse">to reverse list or not.</param>
        /// <returns></returns>
        public IEnumerable<Program> GetRecordedPrograms(long? rule_id = null, int? genre = null, 
            long? channel_id = null, string keyword = null, int limit = 24, int offset = 0, 
            bool reverse = false)
        {
            string EPGATTR = "http://{0}/api/recorded";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            query["reverse"] = reverse.ToString();
            if (rule_id != null) query["rule"] = rule_id.ToString();
            if (genre != null) query["genre1"] = genre.ToString();
            if (channel_id != null) query["channel"] = channel_id.ToString();
            if (keyword != null) query["keyword"] = keyword;
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<RecordList>(body).recorded;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get program! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }

        /// <summary>
        /// Get recorded program's thumbnail image from EPGStation by id.
        /// <para>EPGStation api "/recorded/{id}/thumbnail"</para>
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <param name="length">in case you need a length of the image.</param>
        /// <returns></returns>
        public Image GetRecordedThumbnailByID(int id,out int length)
        {
            length = 0;
            if (id <= 0) return null;
            string EPGATTR = "http://{1}/api/recorded/{0}/thumbnail";
            var req = InitWebRequest(string.Format(EPGATTR, id.ToString(), this.ServiceAddr));
            req.Accept = "image/jpg";
            var resp = GettingResponse(ref req);
            if (resp == null) return null;
            switch ((int)resp.StatusCode)
            {
                case 200:
                    using (System.IO.Stream st = resp.GetResponseStream())
                    {
                        var cLen = resp.Headers.Get("content-length");
                        if (string.IsNullOrWhiteSpace(cLen)) return null;
                        "Receive data[Length:{0}]".ErrorLognConsole(cLen);
                        length = int.Parse(cLen);
                        req.Abort();
                        return Image.FromStream(st);
                    }
                case 404:
                    "Could not find thumbnail of recorde ID [{0}]".ErrorLognConsole(id.ToString());
                    req.Abort();
                    return null;
                default:
                    var err = GetResponseBody(ref resp, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get thumbnail! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get recorded program's thumbnail image from EPGStation by id.
        /// <para>EPGStation api "/recorded/{id}/thumbnail"</para>
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <param name="length">in case you need a length of the image.</param>
        /// <returns></returns>
        public byte[] GetRecordedThumbnailBytesByID(long id)
        {
            if (id <= 0) return null;
            string EPGATTR = "http://{1}/api/recorded/{0}/thumbnail";
            var req = InitWebRequest(string.Format(EPGATTR, id.ToString(), this.ServiceAddr));
            req.Accept = "image/jpg";
            var resp = GettingResponse(ref req);
            if (resp == null) return null;
            switch ((int)resp.StatusCode)
            {
                case 200:
                    using (System.IO.Stream st = resp.GetResponseStream())
                    {
                        var cLen = resp.Headers.Get("content-length");
                        if (string.IsNullOrWhiteSpace(cLen)) return null;
                        "Receive data[Length:{0}]".InfoLognConsole(cLen);
                        byte[] buffer = new byte[int.Parse(cLen)];
                        st.ReadAsync(buffer, 0, int.Parse(cLen));
                        return buffer;
                    }
                case 404:
                    "Could not find thumbnail of recorde ID [{0}]".ErrorLognConsole(id.ToString());
                    req.Abort();
                    return null;
                default:
                    var err = GetResponseBody(ref resp, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get thumbnail! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get recorded program's thumbnail image from EPGStation by id.
        /// <para>EPGStation api "/recorded/{id}/thumbnail"</para>
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns></returns> 
        public Image GetRecordedThumbnailByID(int id)=> this.GetRecordedThumbnailByID(id, out _);
        /// <summary>
        /// Remove Recorded program from server (and local?).
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns>return <see cref="true"/> when success.</returns>
        public bool DeleteRecordByID(long id)
        {
            if (id <= 0) return false;
            string EPGATTR = "http://{1}/api/recorded/{0}";
            var req = InitWebRequest(string.Format(EPGATTR, id.ToString(), this.ServiceAddr),RequestMethods.DELETE);
            HttpWebResponse resp = GettingResponse(ref req);
            if (resp == null) return false;
            switch (resp.StatusCode)
            {
                case HttpStatusCode.OK:
                    "Successfully removed record [RECORDID={0}]".InfoLognConsole(id);
                    return true;
                case HttpStatusCode.Conflict:
                    "Unable to remove record : targer record is not streaming.".ErrorLognConsole();
                    return false;
                default:
                    using (System.IO.Stream st = resp.GetResponseStream())
                    {
                        var cLen = resp.Headers.Get("content-length");
                        if (string.IsNullOrWhiteSpace(cLen)) return false;
                        byte[] buffer = new byte[int.Parse(cLen)];
                        st.ReadAsync(buffer, 0, int.Parse(cLen));
                        string json = System.Text.Encoding.UTF8.GetString(buffer);
                        var tmp = JsonConvert.DeserializeObject<EPGDefault>(json);
                        tmp.errors.InfoLognConsole();
                    }
                    return false;
            }
        }
        #endregion

        #region"API Channel"
        /// <summary>
        /// Get Channel list from EPGStation.
        /// <para>EPGStation api "/channels"</para>
        /// </summary>
        /// <param name="username">EPGStaion Credential's username</param>
        /// <param name="passwd">EPGStaion Credential's pass word</param>
        /// <param name="serviceAddr">server ip address and port. in format 'ip-addr:port'.</param>
        /// <returns></returns>
        public IEnumerable<EPGchannel> GetChannels()
        {
            string EPGATTR = "http://{0}/api/channels";
            var req = InitWebRequest(string.Format(EPGATTR, this.ServiceAddr));
            var resp = GettingResponse(ref req);
            if (resp == null) return null;
            switch ((int)resp.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref resp, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<IEnumerable<EPGchannel>>(body);
                default:
                    var err = GetResponseBody(ref resp, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get program! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }

        /// <summary>
        /// Get Channel name in <see cref="string"/> by ID.
        /// </summary>
        /// <param name="cid">Channel ID</param>
        /// <returns></returns>
        public string GetChannelNameByID(long cid)=> GetChannels()?.GetChannelNameByID(cid);

        /// <summary>
        /// Get channel logo image by Channel ID.
        /// <para>EPGStation api "/channels/{id}/logo"</para>
        /// <para>Untested!!</para>
        /// </summary>
        /// <param name="cid">Channel ID</param>
        /// <returns></returns>
        public Image GetChannelLogoByID(long cid, out int length)
        {
            length = 0;
            string EPGATTR = "http://{0}/api/channels/{1}/logo";
            var req = InitWebRequest(string.Format(EPGATTR, this.ServiceAddr,cid.ToString()));
            req.Accept = "image/png";
            var resp = GettingResponse(ref req);
            if (resp == null) return null;
            switch ((int)resp.StatusCode)
            {
                case 200:
                    using (System.IO.Stream st = resp.GetResponseStream())
                    {
                        var cLen = resp.Headers.Get("content-length");
                        if (string.IsNullOrWhiteSpace(cLen)) return null;
                        Console.WriteLine("Received data from EPGStation server. [Length:{0}]", cLen);
                        "Receive data[Length:{0}]".InfoLognConsole(cLen);
                        length = int.Parse(cLen);
                        return Image.FromStream(st);
                    }
                case 404:
                    "Could not find thumbnail of channel ID [{0}]".ErrorLognConsole(cid.ToString());
                    req.Abort();
                    return null;
                default:
                    var err = GetResponseBody(ref resp, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get thumbnail! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }

        /// <summary>
        /// Get channel logo image by Channel ID.
        /// <para>EPGStation api "/channels/{id}/logo"</para>
        /// <para>Untested!!</para>
        /// </summary>
        /// <param name="cid">Channel ID</param>
        /// <returns></returns>
        public byte[] GetChannelLogoBytesByID(long cid)
        {
            string EPGATTR = "http://{0}/api/channels/{1}/logo";
            var req = InitWebRequest(string.Format(EPGATTR, this.ServiceAddr, cid.ToString()));
            req.Accept = "image/png";
            HttpWebResponse resp = GettingResponse(ref req);
            if (resp == null) return null;
            switch ((int)resp.StatusCode)
            {
                case 200:
                    using (System.IO.Stream st = resp.GetResponseStream())
                    {
                        var cLen = resp.Headers.Get("content-length");
                        if (string.IsNullOrWhiteSpace(cLen)) return null;
                        Console.WriteLine("Received data from EPGStation server. [Length:{0}]", cLen);
                        "Receive data[Length:{0}]".InfoLognConsole(cLen);
                        byte[] buffer = new byte[int.Parse(cLen)];
                        st.ReadAsync(buffer, 0, int.Parse(cLen));
                        return buffer;
                    }
                case 404:
                    "Could not find thumbnail of channel ID [{0}]".ErrorLognConsole(cid.ToString());
                    req.Abort();
                    return null;
                default:
                    var err = GetResponseBody(ref resp, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get thumbnail! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }

        /// <summary>
        /// Get channel logo image by Channel ID.
        /// <para>EPGStation api "/channels/{id}/logo"</para>
        /// <para>Untested!!</para>
        /// </summary>
        /// <param name="cid">Channel ID</param>
        /// <returns></returns>
        public Image GetChannelLogoByID(long cid)=> this.GetChannelLogoByID(cid, out _);
        #endregion

        #region "API Reserves"
        /// <summary>
        /// Get reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves"</para>
        /// </summary>
        /// <param name="count">The count of reserves in this action.</param>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetReserves(out int count, int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            count = 0;
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    count = obj.Count;
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves"</para>
        /// </summary>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetReserves(int offset = 0,int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get all normal reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves"</para>
        /// </summary>
        public List<Reserve> GetReserves()
        {
            List<Reserve> result = new List<Reserve>();
            int total = 0;
            while (true)
            {                
                var tmp = GetReserves(total, 24);
                result.AddRange(tmp);
                total += tmp.Count();
                if (tmp.Count() < 24) break;
            }
            return result;
        }
        /// <summary>
        /// Get reserve from EPGStation server by reserve ID.
        /// <para>EPGStation api GET "/reserves/{id}"</para>
        /// </summary>
        /// <param name="id">The reserve id of this action.</param>
        /// <returns></returns>
        public Reserve GetReserveByID(long id)
        {
            string EPGATTR = "http://{0}/api/reserves/{1}";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr,id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<Reserve>(body);
                case 404:
                    "There was no reservation for this specified program id [{0}]".ErrorLognConsole(id.ToString());
                    req.Abort(); return null;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Add a new reserve using a <see cref="Api.AddReserve"/> object.
        /// <para>EPGStation api POST "/reserves"</para>
        /// </summary>
        /// <param name="program"></param>
        public bool AddReserve(AddReserve program)
        {
            string EPGATTR = "http://{0}/api/reserves";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.POST);
            var jstr = JsonConvert.SerializeObject(program);
            HttpWebResponse rep = GettingResponse(ref req,UTF8Encoding.UTF8.GetBytes(jstr));
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    return true;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return false;
            }
        }
        /// <summary>
        /// Add a new reserve using it's program ID.
        /// <para>EPGStation api POST "/reserves"</para>
        /// </summary>
        public bool AddReserve(long programID)=> AddReserve(new AddReserve { programId = programID });

        /// <summary>
        /// Delete reserve by it's ID.
        /// <para>EPGStation api DELETE "/reserves/{id}"</para>
        /// </summary>
        public bool DeleteReserveByID(long id)
        {
            string EPGATTR = "http://{0}/api/reserves/{1}";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(),RequestMethods.DELETE);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    req.Abort();
                    return true;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();return false;
            }
        }
        /// <summary>
        /// Edit a reserve.
        /// <para>EPGStation api PUT "/reserves/{id}"</para>
        /// </summary>
        /// <param name="id">Program ID.</param>
        /// <param name="editReserve">the reserve edit body object.</param>
        public bool EditReserveByID(long id,EditReserve editReserve)
        {
            string EPGATTR = "http://{0}/api/reserves/{1}";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(),RequestMethods.PUT);
            var jstr = JsonConvert.SerializeObject(editReserve);
            HttpWebResponse rep = GettingResponse(ref req,UTF8Encoding.UTF8.GetBytes(jstr));
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort(); return true;
                case 404:
                    "There was no reservation for this specified program id [{0}]".ErrorLognConsole(id.ToString());
                    req.Abort(); return false;
                case 406:
                    "Unable to edit because the reservation of id [{0}] is issued by rule.".ErrorLognConsole(id.ToString());
                    req.Abort(); return false;
                case 409:
                    "Unable to edit because the reservation of id [{0}] is being recording.".ErrorLognConsole(id.ToString());
                    req.Abort();return false;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to edit reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();return false;
            }
        }

        /// <summary>
        /// Get reserves that marked skip from EPGStation server.
        /// <para>EPGStation api GET "/reserves/skips"</para>
        /// </summary>
        /// <param name="count">The count of reserves in this action.</param>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetSkipedReserves(out int count, int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves/skips";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            count = 0;
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    count = obj.Count;
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get reserves marked skip from EPGStation server.
        /// <para>EPGStation api GET "/reserves/skips"</para>
        /// </summary>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetSkipedReserves(int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves/skips";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get all skiped reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves/skips"</para>
        /// </summary>
        public List<Reserve> GetSkipedReserves()
        {
            List<Reserve> result = new List<Reserve>();
            int total = 0;
            while (true)
            {
                var tmp = GetSkipedReserves(total, 24);
                result.AddRange(tmp);
                total += tmp.Count();
                if (tmp.Count() < 24) break;
            }
            return result;
        }
        /// <summary>
        /// Unmark "skip" to reserve.
        /// <para>EPGStation api GET "/reserves/{id}/skip" 予約除外状態を解除</para>
        /// </summary>
        /// <param name="id"></param>
        public bool UndoSkipedReserve(long id)
        {
            string EPGATTR = "http://{0}/api/reserves/{1}/skip";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.DELETE);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort(); return true;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to edit reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort(); return false;
            }
        }
        /// <summary>
        /// Get overlaped reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves/overlaps"</para>
        /// </summary>
        /// <param name="count">The count of reserves in this action.</param>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetOverlapedReserves(out int count, int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves/overlaps";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            count = 0;
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    count = obj.Count;
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get overlaped reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves/overlaps"</para>
        /// </summary>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetOverlapedReserves(int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves/overlaps";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get all overlaped reserves from EPGStation server.
        /// <para>EPGStation api GET "/reserves/overlaps"</para>
        /// </summary>
        public List<Reserve> GetOverlapedReserves()
        {
            List<Reserve> result = new List<Reserve>();
            int total = 0;
            while (true)
            {
                var tmp = GetOverlapedReserves(total, 24);
                result.AddRange(tmp);
                total += tmp.Count();
                if (tmp.Count() < 24) break;
            }
            return result;
        }
        /// <summary>
        /// Unmark "Overlaped" to reserve.
        /// <para>EPGStation api GET "/reserves/{id}/overlap"
        /// overlap を解除</para>
        /// </summary>
        /// <param name="id"></param>
        public bool UndoOverlapedReserve(long id)
        {
            string EPGATTR = "http://{0}/api/reserves/{1}/overlap";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.DELETE);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort(); return true;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to edit reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort(); return false;
            }
        }
        /// <summary>
        /// Get reserve conflicts from EPGStation server.
        /// <para>EPGStation api GET "/reserves/conflicts"</para>
        /// </summary>
        /// <param name="count">The count of reserves in this action.</param>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetReserveConflicts(out int count, int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves/conflicts";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            count = 0;
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    count = obj.Count;
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get reserve conflicts from EPGStation server.
        /// <para>EPGStation api GET "/reserves/conflicts"</para>
        /// </summary>
        /// <param name="limit">The maxiun count of reserve items.</param>
        /// <param name="offset">The offset of request.</param>
        /// <returns></returns>
        public IEnumerable<Reserve> GetReserveConflicts(int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/reserves/conflicts";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Reserve>.DeserializObject(body);
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get all reserve conflicts from EPGStation server.
        /// <para>EPGStation api GET "/reserves/conflicts"</para>
        /// </summary>
        public List<Reserve> GetReserveConflicts()
        {
            List<Reserve> result = new List<Reserve>();
            int total = 0;
            while (true)
            {
                var tmp = GetReserveConflicts(total, 24);
                result.AddRange(tmp);
                total += tmp.Count();
                if (tmp.Count() < 24) break;
            }
            return result;
        }
        /// <summary>
        /// Get all reserve's program id.
        /// <para>EPGStation api GET "/reserves/all"</para>
        /// </summary>
        public ReserveAllId GetAllReserveIDs()
        {
            string EPGATTR = "http://{0}/api/reserves/all";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<ReserveAllId>(body);
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        #endregion

        #region "API Rules"
        /// <summary>
        /// Get rule list.
        /// <para>EPGStation api "/rules/list"</para>
        /// </summary>
        public IEnumerable<RuleList> GetRuleList()
        {
            string EPGATTR = "http://{0}/api/rules/list";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<IEnumerable<RuleList>>(body);
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get program! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get rules from epgstation server.
        /// <para>EPGStation api "/rules"</para>
        /// </summary>
        /// <param name="count">total rule count.</param>
        /// <param name="offset">offset of this action.</param>
        /// <param name="limit">max item of this action.</param>
        public IEnumerable<Rule> GetRules(out int count,int offset =0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/rules";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            count = 0;
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Rule>.DeserializObject(body);
                    count = obj.Count;
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get rules from epgstation server.
        /// <para>EPGStation api "/rules"</para>
        /// </summary>
        /// <param name="offset">offset of this action.</param>
        /// <param name="limit">max item of this action.</param>
        public IEnumerable<Rule> GetRules(int offset = 0, int limit = 24)
        {
            string EPGATTR = "http://{0}/api/rules";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["limit"] = limit.ToString();
            query["offset"] = offset.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var obj = Collection<Rule>.DeserializObject(body);
                    return obj.Items;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to get reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get all rules from epgstation server.
        /// <para>EPGStation api "/rules"</para>
        /// </summary>
        public List<Rule> GetRules()
        {
            List<Rule> result = new List<Rule>();
            int total = 0;
            while (true)
            {
                var tmp = GetRules(total, 24);
                result.AddRange(tmp);
                total += tmp.Count();
                if (tmp.Count() < 24) break;
            }
            return result;
        }
        /// <summary>
        /// Add a new rule using a <see cref="Api.AddRule"/> object.
        /// <para>EPGStation api POST "/rules"</para>
        /// </summary>
        /// <param name="rule"></param>
        public int AddRule(AddRule rule)
        {
            string EPGATTR = "http://{0}/api/rules";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.POST);
            var jstr = JsonConvert.SerializeObject(rule);
            HttpWebResponse rep = GettingResponse(ref req, UTF8Encoding.UTF8.GetBytes(jstr));
            if (rep == null) return -1;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    JObject obj = (JObject)JsonConvert.DeserializeObject(body);
                    return (int)obj.GetValue("id");
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return -1;
            }
        }
        /// <summary>
        /// Enable a rule by it's ID.
        /// <para>EPGStation api PUT "/rules/{id}/enable"</para>
        /// </summary>
        /// <param name="id">Rule ID</param>
        /// <returns></returns>
        public bool EnableRule(int id)
        {
            string EPGATTR = "http://{0}/api/rules/{1}/enable";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr,id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.PUT);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort();
                    return true;
                case 404:
                    req.Abort();
                    "Rule ID [{0}]: No such rule!".ErrorLognConsole(id.ToString());
                    return false;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return false;
            }
        }
        /// <summary>
        /// Disable a rule by it's ID.
        /// <para>EPGStation api PUT "/rules/{id}/disable"</para>
        /// </summary>
        /// <param name="id">Rule ID</param>
        /// <returns></returns>
        public bool DisableRule(int id)
        {
            string EPGATTR = "http://{0}/api/rules/{1}/disable";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.PUT);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort();
                    return true;
                case 404:
                    req.Abort();
                    "Rule ID [{0}]: No such rule!".ErrorLognConsole(id.ToString());
                    return false;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return false;
            }
        }
        /// <summary>
        /// Get a rule by it's ID.
        /// <para>EPGStation api GET "/rules/{id}"</para>
        /// </summary>
        /// <param name="id">Rule ID</param>
        /// <returns></returns>
        public Rule GetRuleByID(int id)
        {
            string EPGATTR = "http://{0}/api/rules/{1}";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    var body = GetResponseBody(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<Rule>(body);
                case 404:
                    req.Abort();
                    "Rule ID [{0}]: No such rule!".ErrorLognConsole(id.ToString());
                    return null;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Edit a rule.
        /// <para>EPGStation api PUT "/rules/{id}"</para>
        /// </summary>
        /// <param name="id">Rule ID</param>
        /// <returns></returns>
        public bool EditRuleByID(int id,AddRule rule)
        {
            string EPGATTR = "http://{0}/api/rules/{1}";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(),RequestMethods.PUT);
            var jstr = JsonConvert.SerializeObject(rule);
            var rep = GettingResponse(ref req, UTF8Encoding.UTF8.GetBytes(jstr));
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort();
                    return true;
                case 400:
                    req.Abort();
                    "Rule ID [{0}]: 入力に間違えがある場合".ErrorLognConsole(id.ToString());
                    return false;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return false;
            }
        }
        /// <summary>
        /// Delete a rule.
        /// <para>EPGStation api DELETE "/rules/{id}"</para>
        /// </summary>
        /// <param name="id">Rule ID</param>
        /// <returns></returns>
        public bool DeleteRuleByID(int id)
        {
            string EPGATTR = "http://{0}/api/rules/{1}";
            var ub = new UriBuilder(string.Format(EPGATTR, this.ServiceAddr, id.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            var req = InitWebRequest(ub.ToString(), RequestMethods.DELETE);
            var rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                case 201:
                    req.Abort();
                    return true;
                default:
                    var err = GetResponseBody(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<EPGDefault>(err);
                    "Failed to add reserve! [{0}] reason: {1}".ErrorLognConsole(json.code, json.message);
                    req.Abort();
                    return false;
            }
        }
        #endregion


    }
}
