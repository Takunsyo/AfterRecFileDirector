using Newtonsoft.Json;
using RVMCore.EPGStationWarpper.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        #region "API Recored"
        /// <summary>
        /// Get Recorded program by record ID.
        /// <para>EPGStation api "/recorded/{id}"</para>
        /// </summary>
        /// <param name="id">Record ID</param>
        /// <returns></returns>
        public RecordedProgram GetRecordProgramByID(int id)
        {
            if (id == null || id <= 0) return null;
            string EPGATTR = "http://{1}/api/recorded/{0}";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format(EPGATTR, id.ToString(), this.ServiceAddr));
            req.Method = "GET";
            req.Credentials = this.EPGCredential;
            HttpWebResponse resp;
            try
            {
                Console.WriteLine("curl -X GET \"{0}\" -H \"accept: application/json\"",req.RequestUri);
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch(WebException ex)
            {
                Console.WriteLine("Catch Error while GET api from server:");
                Console.WriteLine(ex.Message);
                return null;
            }
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                Console.WriteLine("Received data from EPGStation server. [Length:{0}]", cLen);
                byte[] buffer = new byte[int.Parse(cLen)];
                st.ReadAsync(buffer, 0, int.Parse(cLen));
                string tmp = System.Text.Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<RecordedProgram>(tmp);
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
        public IEnumerable<RecordedProgram> GetRecordedPrograms(long rule_id = -1, int genre = -1, long channel_id = -1, string keyword = "", int limit = 24, int offset = 0, bool reverse = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("http://{0}/api/recorded", this.ServiceAddr);
            sb.AppendFormat("?limit={0}&offset={1}&reverse={2}", limit.ToString(), offset.ToString(), reverse.ToString());
            if (rule_id >= 0) sb.AppendFormat("&rule={0}", rule_id.ToString());
            if (genre >= 0) sb.AppendFormat("&genre1={0}", genre.ToString());
            if (channel_id >= 0) sb.AppendFormat("&channel={0}", channel_id.ToString());
            if (!keyword.IsNullOrEmptyOrWhiltSpace()) sb.AppendFormat("&keyword={0}", keyword);
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sb.ToString());
            req.Method = "GET";
            req.Credentials = this.EPGCredential;
            HttpWebResponse resp;
            try
            {
                Console.WriteLine("curl -X GET \"{0}\" -H \"accept: application/json\"", req.RequestUri);
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine("Catch Error while GET api from server:");
                Console.WriteLine(ex.Message);
                return null;
            }
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                Console.WriteLine("Received data from EPGStation server. [Length:{0}]", cLen);
                byte[] buffer = new byte[int.Parse(cLen)];
                st.ReadAsync(buffer, 0, int.Parse(cLen));
                string tmp = System.Text.Encoding.UTF8.GetString(buffer);
                return JsonConvert.DeserializeObject<RecordList>(tmp).recorded;
            }
        }

        /// <summary>
        /// Get recorded program's thumbnail image from EPGStation by id.
        /// <para>EPGStation api "/recorded/{id}/thumbnail"</para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="length">in case you need a length of the image.</param>
        /// <returns></returns>
        public Image GetRecordedThumbnailByID(int id,out int length)
        {
            length = 0;
            if (id <= 0) return null;
            string EPGATTR = "http://{1}/api/recorded/{0}/thumbnail";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format(EPGATTR, id.ToString(), this.ServiceAddr));
            req.Accept = "image/jpg";
            req.Method = "GET";
            req.Credentials = this.EPGCredential;
            HttpWebResponse resp;
            try
            {
                Console.WriteLine("curl -X GET \"{0}\" -H \"accept: image/jpg\"", req.RequestUri);
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine("Catch Error while GET api from server:");
                Console.WriteLine(ex.Message);
                return null;
            }
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                length = int.Parse(cLen);
                return Image.FromStream(st);
            }
        }

        /// <summary>
        /// Get recorded program's thumbnail image from EPGStation by id.
        /// <para>EPGStation api "/recorded/{id}/thumbnail"</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Image GetRecordedThumbnailByID(int id)
        {
            int length = 0;
            return this.GetRecordedThumbnailByID(id, out length);
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
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format(EPGATTR, this.ServiceAddr));
            req.Method = "GET";
            req.Credentials = this.EPGCredential;
            //req.ContentType = "application/json; charset=utf-8";
            HttpWebResponse resp;
            try
            {
                Console.WriteLine("curl -X GET \"{0}\" -H \"accept: application/json\"", req.RequestUri);
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine("Catch Error while GET api from server:");
                Console.WriteLine(ex.Message);
                return null;
            }
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                Console.WriteLine("Received data from EPGStation server. [Length:{0}]", cLen);
                byte[] buffer = new byte[int.Parse(cLen)];
                st.ReadAsync(buffer, 0, int.Parse(cLen));
                string tmp = System.Text.Encoding.UTF8.GetString(buffer);
                var mp = JsonConvert.DeserializeObject<IEnumerable<EPGchannel>>(tmp);
                return mp;
            }
        }

        /// <summary>
        /// Get Channel name in <see cref="string"/> by ID.
        /// </summary>
        /// <param name="cid">Channel ID</param>
        /// <returns></returns>
        public string GetChannelNameByID(long cid)
        {
            return GetChannels().GetChannelNameByID(cid);
        }

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
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format(EPGATTR, this.ServiceAddr,cid.ToString()));
            req.Method = "GET";
            req.Credentials = this.EPGCredential;
            req.Accept = "image/png";
            HttpWebResponse resp;
            try
            {
                Console.WriteLine("curl -X GET \"{0}\" -H \"accept: image/png\"", req.RequestUri);
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                Console.WriteLine("Catch Error while GET api from server:");
                Console.WriteLine(ex.Message);
                return null;
            }
            using (System.IO.Stream st = resp.GetResponseStream())
            {
                var cLen = resp.Headers.Get("content-length");
                if (string.IsNullOrWhiteSpace(cLen)) return null;
                Console.WriteLine("Received data from EPGStation server. [Length:{0}]", cLen);
                length = int.Parse(cLen);
                //st.Position = 0;
                return Image.FromStream(st);
            }
        }

        /// <summary>
        /// Get channel logo image by Channel ID.
        /// <para>EPGStation api "/channels/{id}/logo"</para>
        /// <para>Untested!!</para>
        /// </summary>
        /// <param name="cid">Channel ID</param>
        /// <returns></returns>
        public Image GetChannelLogoByID(long cid)
        {
            int len = 0;
            return this.GetChannelLogoByID(cid, out len);
        }
        #endregion

    }
}
