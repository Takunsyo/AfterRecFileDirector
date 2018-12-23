using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RVMCore.MirakurunWarpper.Apis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Drawing;

namespace RVMCore.MirakurunWarpper
{
    /// <summary>
    /// This class provides methods to communicate with Mirakurun Server.
    /// </summary>
    public class MirakurunService:IDisposable
    {
        /*This class supports Mirakurun Ver 2.7.3*/
        /// <summary>
        /// The uri direct to Mirakurun Server root.
        /// </summary>
        public Uri ServiceAddr { get; private set; }

        public ServerVersion Version { get; private set; }

        #region Static Methods
        /// <summary>
        /// Get a UNIX Timestamp from a specific DateTime object.
        /// <para>*TimeStamp is counted in milliseconds.</para>
        /// </summary>
        public static long GetUNIXTimeStamp(DateTime time)
        {
            return  (long)(time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }

        /// <summary>
        /// Get current time UNIX Timestamp.
        /// <para>*TimeStamp is counted in milliseconds.</para>
        /// </summary>
        public static long GetUNIXTimeStamp()
        {
            return GetUNIXTimeStamp(DateTime.Now);
        }

        /// <summary>
        /// Get a <see cref="DateTime"/> object from a UNIX TimeStamp.
        /// </summary>
        /// <param name="UNIXTimeStamp">The TimeStamp are Mirakurun server is using counted in milliseconds.</param>
        /// <returns></returns>
        public static DateTime GetDateTime(long UNIXTimeStamp)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(UNIXTimeStamp);
        }
        #endregion

        #region Init
        /// <summary>
        /// Initialize a new instance of <see cref="MirakurunService"/>
        /// with <see cref="SettingObj"/> specify a setting include the
        /// address of Mirakurun Server.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException">
        /// Thrown when setting doesn't include server address.</exception>
        /// <exception cref="WebException">Throw when server returns error.</exception>
        /// <param name="setting">
        /// A setting include a address direct to Mirakurun Server</param>
        public MirakurunService(SettingObj setting)
        {
            if (setting is null) throw new ArgumentNullException();
            if (setting.Mirakurun_ServiceAddr.IsNullOrEmptyOrWhiltSpace())
                throw new ArgumentException("Invalid Setting: /n Setting doesn't include server address.");
            this.ServiceAddr = new Uri(setting.Mirakurun_ServiceAddr);
            Exception e;
            this.Version = this.GetServerVersion(out e);
            if (e != null) throw e;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MirakurunService"/>
        /// with a uri string direct to Mirakurun server root.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when ServAddr is null or is a invalid uri string.</exception>
        /// <exception cref="WebException">Throw when server returns error.</exception>
        /// <param name="ServAddr">A uri string direct to Mirakurun server root.</param>
        public MirakurunService(string ServAddr)
        {
            if (ServAddr.IsNullOrEmptyOrWhiltSpace())
                throw new InvalidOperationException("Address is null.");
            if (!Uri.IsWellFormedUriString(ServAddr, UriKind.RelativeOrAbsolute))
                throw new InvalidOperationException("Uri string is invalid.");
            this.ServiceAddr = new Uri(ServAddr);
            Exception e;
            this.Version = this.GetServerVersion(out e);
            if (e != null) throw e;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MirakurunService"/>
        /// with a <see cref="Uri"/> direct to Mirakurun server root.
        /// </summary>
        /// <exception cref="WebException">Throw when server returns error.</exception>
        /// <param name="ServAddr">Uri to Mirakurun server root.</param>
        public MirakurunService(Uri ServAddr)
        {
            this.ServiceAddr = ServiceAddr;
            Exception e;
            this.Version = this.GetServerVersion(out e);
            if (e != null) throw e;
        }
        #endregion

        #region private methods 

        private enum RequestMethods {
            GET,
            PUT,
            DELETE
        }

        private static HttpWebRequest InitWebRequest(Uri uri,RequestMethods method = RequestMethods.GET)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = "RVMCore.MirakurunWatcher/" + Environment.Version;
            switch (method) {
                case RequestMethods.GET:
                    req.Method = "GET";
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
            return req;
        }

        private static HttpWebRequest InitWebRequest(string uri, RequestMethods method = RequestMethods.GET)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = "RVMCore.MirakurunWatcher/" + Environment.Version ;
            switch (method)
            {
                case RequestMethods.GET:
                    req.Method = "GET";
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
            return req;
        }

        private static HttpWebResponse GettingResponse(ref HttpWebRequest request)=>GettingResponse(ref request,out _);

        private static HttpWebResponse GettingResponse(ref HttpWebRequest request , out Exception e)
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

        private static string GetResponseBodyString(ref HttpWebResponse response,Encoding encoding, bool CloseAfterWork = true)
        {
            if ((response?.ContentLength ?? 0) <= 0 ) return null;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                var tmp = reader.ReadToEnd();
                if(CloseAfterWork)response.Close();
                return tmp;
            }
        }
        #endregion

        //Apis:

        #region"programs"
        /// <summary>
        /// Get Programs.<para>Mirakurun API : "/programs"</para>
        /// </summary>
        /// <param name="networkID">networkId</param>
        /// <param name="serviceID">serviceId</param>
        /// <param name="eventID">eventId</param>
        /// <returns>A array of <see cref="Program"/></returns>
        public IEnumerable<Program> GetPrograms(int? networkID=null, int? serviceID=null,long? eventID=null)
        {
            var ub = new UriBuilder( ServiceAddr.ToString()+ "api/programs");
            var query = HttpUtility.ParseQueryString(ub.Query);
            if(networkID != null)query["networkId"] = networkID.ToString();
            if(serviceID != null)query["serviceId"] = serviceID.ToString();
            if(eventID !=null)query["eventId"] = eventID.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode) {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8,true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<IEnumerable<Program>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get programs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    
                    return null;
            }
        }
        /// <summary>
        /// Get program infomation by program ID.
        /// <para>Mirakurun API : "/programs/{id}"</para>
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>A <see cref="Program"/> object.</returns>
        public Program GetProgramByID(long id)
        {
            Uri ub = new Uri(ServiceAddr, "api/programs/" + id.ToString());
            var req = InitWebRequest(ub);
            var rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    
                    return JsonConvert.DeserializeObject<Program>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get programs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    return null;
            }
        }
        /// <summary>
        /// Initialize a <see cref="Thread"/> that record certain program's TransportStream from Mirakurun to File.
        /// <para>Mirakurun API: "GET /programs/{id}/stream"</para>
        /// </summary>
        /// <param name="programId">program Id</param>
        /// <param name="path">Path to the file to record.</param>
        /// <param name="cancelToken">A token to stop record operation can be provide by <see cref="CancellationTokenSource"/>.</param>
        /// <param name="decode"></param>
        /// <param name="priority"></param>
        /// <returns>A <see cref="Thread"/> can be started.</returns>
        public Thread StreamProgramToFile(long programId, string path, CancellationToken cancelToken,
                                     int? decode = null, StreamPriority priority = StreamPriority.Default)
        {
            var workLoad = new ThreadStart(new Action(() => {
                if (System.IO.File.Exists(path)) return;
                var ub = new UriBuilder(ServiceAddr.ToString() + string.Format("api/programs/{0}/stream", programId.ToString()));
                var query = HttpUtility.ParseQueryString(ub.Query);
                if (decode != null) query["decode"] = decode.ToString();
                ub.Query = query.ToString();
                var req = InitWebRequest(ub.ToString());
                req.Headers.Add("X-Mirakurun-Priority", ((int)priority).ToString());
                var rep = GettingResponse(ref req);
                if (rep == null) return;
                switch ((int)rep.StatusCode)
                {
                    case 200:
                        using (var writer = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                        {
                            using (var input = rep.GetResponseStream())
                            {
                                byte[] buffer = new byte[188 * 100];
                                int bytesRead;
                                try
                                {
                                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        writer.Write(buffer, 0, bytesRead);
                                        cancelToken.ThrowIfCancellationRequested();
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    Console.WriteLine("Operation canceled!");
                                }
                                catch (Exception e)
                                {
                                    e.Message.ErrorLognConsole();
                                }
                            }
                        }
                        break;
                    //Write to file.
                    case 404:
                        "cannot find chanel. Code:404".ErrorLognConsole();
                        break;
                    case 503:
                        "Tuner Resource Unavailable. Code:503".ErrorLognConsole();
                        break;
                    default:
                        var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                        var json = JsonConvert.DeserializeObject<@default>(mbody);
                        "Failed to get program stream! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                        break;
                }
                req.Abort(); rep.Close();
            }));
            return new Thread(workLoad);
        }
        /// <summary>
        /// Get a path can provide stream service from Mirakurun server.
        /// <para>Mirakurun API: "GET /programs/{id}/stream"</para>
        /// </summary>
        /// <param name="serviceId">program Id</param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public string GetProgramStreamPath(long programId,int? decode = null)
        {
            var ub = new UriBuilder(ServiceAddr.ToString() + string.Format("api/programs/{0}/stream", programId.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            if (decode != null) query["decode"] = decode.ToString();
            ub.Query = query.ToString();
            return ub.ToString();
        }
        #endregion

        #region"channels"
        /// <summary>
        /// Get TS Channels(as transponders) information.
        /// <para>Mirakurun API : "/channels"</para>
        /// <para>This API works as same as Mirakurun API : "/channels/{type}"</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Channel> GetChannels(ChannelType type, int channel =-1 , string name = "")
        {
            var ub = new UriBuilder(ServiceAddr.ToString()+ "api/channels");
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["type"] = type.ToString();
            if(channel >= 0) query["channel"] = channel.ToString();
            if(!name.IsNullOrEmptyOrWhiltSpace())query["name"] = name;
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    
                    return JsonConvert.DeserializeObject<IEnumerable<Channel>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get channels! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    return null;
            }
        }
        /// <summary>
        /// Get a Channel(as transponder) Infotmation.
        /// <para>Mirakurun API:"/channels/{type}/{channel}"</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public Channel GetChannel(ChannelType type, int channel)
        {
            Uri ub = new Uri(ServiceAddr, string.Format("api/channels/{0}/{1}",
                                type.ToString(),
                                channel.ToString()));
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<Channel>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get channel! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }
        /// <summary>
        /// Get a Service(as TVStation) Information. using specific channel ID and service ID.
        /// <para>Mirakurun API:"/channels/{type}/{channel}/sercices/{id}"</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Service GetServiceByID(ChannelType type, int channel, int id)
        {

            Uri ub = new Uri(ServiceAddr, string.Format("/api/channels/{0}/{1}/services/{2}",
                                            type.ToString(),
                                            channel.ToString(),
                                            id.ToString()));
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort(); 
                    return JsonConvert.DeserializeObject<Service>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get service! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }
        /// <summary>
        /// Get a list of service(as TVStations) under a specific channel.
        /// <para>Mirakurun API:"/channels/{type}/{channel}/services"</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public IEnumerable<Service> GetServicesByChannel(ChannelType type,int channel)
        {
            Uri ub = new Uri(ServiceAddr, string.Format("/api/channels/{0}/{1}/services",
                                type.ToString(),
                                channel.ToString()));
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort(); 
                    return JsonConvert.DeserializeObject<IEnumerable<Service>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get service! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }

        /// <summary>
        /// Initialize a <see cref="Thread"/> that record TransportStream from Mirakurun to File.
        /// <para>Mirakurun API:"GET /channels/{type}/{channel}/stream"</para>
        /// <para>Mirakurun API:"GET /channels/{type}/{channel}/services/{id}/stream"</para>
        /// </summary>
        /// <param name="type">Channel types defined in <see cref="ChannelType"/></param>
        /// <param name="cannelId">Channel ID.</param>
        /// <param name="path">Path to the file to record.</param>
        /// <param name="cancelToken">A token to stop record operation can be provide by <see cref="CancellationTokenSource"/>.</param>
        /// <param name="serviceID">Service ID if available</param>
        /// <param name="decode"></param>
        /// <param name="priority"></param>
        /// <returns>A <see cref="Thread"/> can be started.</returns>
        public Thread StreamChannelToFile(ChannelType type, int cannelId , string path, CancellationToken cancelToken,
                                            long? serviceID = null, int? decode = null, StreamPriority priority = StreamPriority.Default)
        {
            var workLoad = new ThreadStart(new Action(() => {
                if (System.IO.File.Exists(path)) return;
                string qPath;
                if(serviceID == null)
                {
                    qPath = string.Format("api/channels/{0}/{1}/stream", type.ToString(), cannelId.ToString());
                }
                else
                {
                    qPath = string.Format("api/channels/{0}/{1}/services/{2}/stream", type.ToString(), cannelId.ToString(),serviceID.ToString());
                }
                var ub = new UriBuilder(ServiceAddr.ToString() + qPath);
                var query = HttpUtility.ParseQueryString(ub.Query);
                if(decode != null)query["decode"] = decode.ToString();
                ub.Query = query.ToString();
                var req = InitWebRequest(ub.ToString());
                req.Headers.Add("X-Mirakurun-Priority", ((int)priority).ToString());
                var rep = GettingResponse(ref req);
                if (rep == null) return;
                switch ((int)rep.StatusCode)
                {
                    case 200:
                        using(var writer = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                        {
                            using(var input = rep.GetResponseStream())
                            {
                                byte[] buffer = new byte[188*100];
                                int bytesRead;
                                try { 
                                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        writer.Write(buffer, 0, bytesRead);
                                        cancelToken.ThrowIfCancellationRequested();
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    Console.WriteLine("Operation canceled!");
                                }
                                catch(Exception e)
                                {
                                    e.Message.ErrorLognConsole();
                                }
                            }
                        }
                        break;
                    //Write to file.
                    case 404:
                        "cannot find chanel. Code:404".ErrorLognConsole();
                        break;
                    case 503:
                        "Tuner Resource Unavailable. Code:503".ErrorLognConsole();
                        break;
                    default:
                        var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                        var json = JsonConvert.DeserializeObject<@default>(mbody);
                        "Failed to get channel stream! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                        break;
                }
                req.Abort(); rep.Close();
            }));
            return new Thread(workLoad);
        }

        /// <summary>
        /// Get a path can provide stream service from Mirakurun server.
        /// <para>Mirakurun API:"GET /channels/{type}/{channel}/stream"</para>
        /// <para>Mirakurun API:"GET /channels/{type}/{channel}/services/{id}/stream"</para>
        /// </summary>
        /// <param name="type">Channel types defined in <see cref="ChannelType"/></param>
        /// <param name="cannelId">Channel ID.</param>
        /// <param name="serviceID">Service ID if available</param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public string GetCannelStreamPath(ChannelType type, int cannelId, long? serviceID = null, int? decode = null)
        {
            string qPath;
            if (serviceID == null)
            {
                qPath = string.Format("api/channels/{0}/{1}/stream", type.ToString(), cannelId.ToString());
            }
            else
            {
                qPath = string.Format("api/channels/{0}/{1}/services/{2}/stream", type.ToString(), cannelId.ToString(), serviceID.ToString());
            }
            var ub = new UriBuilder(ServiceAddr.ToString() + qPath);
            var query = HttpUtility.ParseQueryString(ub.Query);
            if (decode != null) query["decode"] = decode.ToString();
            ub.Query = query.ToString();
            return ub.ToString();
        }

        #endregion

        #region"config"
        /// <summary>
        /// Get tunner config infomations.
        /// <para>Mirakurun API:"/config/tuners" Method "GET"</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TunerConfig> GetTunnerConfigs()
        {
            Uri ub = new Uri(ServiceAddr, "api/config/tuners");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort(); 
                    return JsonConvert.DeserializeObject<IEnumerable<TunerConfig>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get configs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }
        /// <summary>
        /// Set tunner config informations.
        /// <para>Mirakurun API:"/config/tuners" Method "PUT"</para>
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public bool SetTunnerConfigs(IEnumerable<TunerConfig> configs)
        {
            Uri ub = new Uri(ServiceAddr, "/api/config/tuners");
            var req = InitWebRequest(ub,RequestMethods.PUT);
            using(var sw = req.GetRequestStream())
            {
                var body = JsonConvert.SerializeObject(configs);
                var buffer = Encoding.UTF8.GetBytes(body);
                sw.Write(buffer, 0, buffer.Length);
            }
            var rep = GettingResponse(ref req);
            switch ((int)rep.StatusCode)
            {
                case 200:
                    req.Abort(); 
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to set configs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return false;
            }
        }

        /// <summary>
        /// Get server config infomations.
        /// <para>Mirakurun API:"/config/server" Method "GET"</para>
        /// </summary>
        /// <returns></returns>
        public ServerConfig GetServerConfigs()
        {
            Uri ub = new Uri(ServiceAddr, "/api/config/server");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort(); 
                    return JsonConvert.DeserializeObject<ServerConfig>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get server configs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }
        /// <summary>
        /// Set server config informations.
        /// <para>Mirakurun API:"/config/server" Method "PUT"</para>
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public bool SetServerConfigs(ServerConfig configs)
        {
            Uri ub = new Uri(ServiceAddr, "/api/config/server");
            var req = InitWebRequest(ub, RequestMethods.PUT);
            using (var sw = req.GetRequestStream())
            {
                var body = JsonConvert.SerializeObject(configs);
                var buffer = Encoding.UTF8.GetBytes(body);
                sw.Write(buffer, 0, buffer.Length);
            }
            var rep = GettingResponse(ref req);
            switch ((int)rep.StatusCode)
            {
                case 200:
                    req.Abort(); 
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to set server config! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return false;
            }
        }
        /// <summary>
        /// Tell mirakurun server to start channel scan. looks it only work for <see cref="ChannelType.GR"/>.
        /// <para>Mirakurun API:"/config/channels/scan" Method "PUT"</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool StartChannelScan(ChannelType type = ChannelType.GR, uint min = 13, uint max = 52)
        {
            var ub = new UriBuilder(ServiceAddr.ToString()+"api/config/channels/scan");
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["type"] = type.ToString();
            query["min"] = min.ToString();
            query["max"] = max.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString(),RequestMethods.PUT);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    req.Abort(); 
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to start channel scan! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return false;
            }
        }
        /// <summary>
        /// Get channel config infomations.
        /// <para>Mirakurun API:"/config/tuners" Method "GET"</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ChannelConfig> GetChannelConfigs()
        {
            Uri ub = new Uri(ServiceAddr, "/api/config/channels");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort(); 
                    return JsonConvert.DeserializeObject<IEnumerable<ChannelConfig>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get channel configs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }
        /// <summary>
        /// Set channel config informations.
        /// <para>Mirakurun API:"/config/tuners" Method "PUT"</para>
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public bool SetChannelConfigs(IEnumerable<ChannelConfig> configs)
        {
            Uri ub = new Uri(ServiceAddr, "api/config/channels");
            var req = InitWebRequest(ub, RequestMethods.PUT);
            using (var sw = req.GetRequestStream())
            {
                var body = JsonConvert.SerializeObject(configs);
                var buffer = Encoding.UTF8.GetBytes(body);
                sw.Write(buffer, 0, buffer.Length);
            }
            var rep = GettingResponse(ref req);
            switch ((int)rep.StatusCode)
            {
                case 200:
                    req.Abort(); 
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to set channel config! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return false;
            }
        }
        #endregion

        #region"events"
        /// <summary>
        /// Get Events.
        /// <para>Mirakurun API : "/events"</para>
        /// </summary>
        /// <returns>A array of <see cref="Event"/></returns>
        public IEnumerable<Event> GetEvents()
        {
            Uri ub = new Uri(ServiceAddr, "api/events");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort(); 
                    return JsonConvert.DeserializeObject<IEnumerable<Event>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get events! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return null;
            }
        }
        /// <summary>
        /// Show if is event informations being watched.
        /// </summary>
        public bool IsEventSubscribed => eventSubscribeThread?.IsAlive ?? false;
        /// <summary>
        /// Current watching event's type.
        /// </summary>
        public EventType? SubscribedEventType { get; private set; } = null;
        /// <summary>
        /// Current watching event's resource type.
        /// </summary>
        public ResourceType? SubscribedResourceType { get; private set; } = null;
        private Thread eventSubscribeThread;
        private CancellationTokenSource eventSubscribeCancellationTokenSource;
        /// <summary>
        /// This event will be raised when event informations from Mirakurun server has been recived.
        /// </summary>
        public event EventRecivedHandler EventRecived;
        /// <summary>
        /// Start subscribe informations.
        /// <para>When information recived a <see cref="EventRecived"/> event will be raised.</para>
        /// GET /events/stream
        /// </summary>
        /// <param name="FailedCallback">Callback delegate in case of failed subscribe.</param>
        /// <param name="type"></param>
        /// <param name="resource"></param>
        public void SubscribeEvents(EventType? type = null,ResourceType? resource = null)
        {
            if (!(eventSubscribeThread is null) && eventSubscribeThread.IsAlive) return;
            var workLoad = new ThreadStart(new Action(() => {
                SubscribeEventsWork(0, type, resource);
            }));
            eventSubscribeCancellationTokenSource = new CancellationTokenSource();
            eventSubscribeThread = new Thread(workLoad);
            eventSubscribeThread.IsBackground = true;
            eventSubscribeThread.Start();
        }

        private void SubscribeEventsWork(int SleepTime = 0, EventType? type = null, ResourceType? resource = null)
        {
            Thread.Sleep(SleepTime);
            var ub = new UriBuilder(ServiceAddr.ToString() + "api/events/stream");
            var query = HttpUtility.ParseQueryString(ub.Query);
            if (type != null) query["type"] = type.ToString();
            if (resource != null) query["resource"] = resource.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            req.Timeout = Timeout.Infinite;
            var rep = GettingResponse(ref req);
            if (rep == null) { return; }
            switch ((int)rep.StatusCode)
            {
                case 200:
                    GetMessages();
                    break;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get event stream! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    SubscribeEventsWork(3000, type, resource);
                    break;
            }
            void GetMessages()
            {
                using (StreamReader streadReader = new StreamReader(rep.GetResponseStream(), Encoding.UTF8))
                {

                    streadReader.BaseStream.ReadTimeout = Timeout.Infinite;
                    SubscribedEventType = type ?? (EventType.create | EventType.redefine | EventType.update);
                    SubscribedResourceType = resource ?? (ResourceType.program | ResourceType.service | ResourceType.tuner);
                    //Start parsing cycle
                    try
                    {
                        while (!streadReader.EndOfStream)
                        {
                            eventSubscribeCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            string resultLine = streadReader.ReadLine();
                            //System.Diagnostics.Trace.WriteLine(resultLine);
                            if (!resultLine.StartsWith("{")) continue;
                            var tmp = JsonConvert.DeserializeObject<Event>(resultLine);
                            this.EventRecived?.Invoke(this, tmp);
                        }
                        req.Abort(); rep.Close(); rep.Dispose();
                        SubscribeEventsWork(3000, type, resource);
                    }
                    catch (OperationCanceledException)
                    {
                        req.Abort(); rep.Close(); rep.Dispose();
                        "Stoped subsctiption.".InfoLognConsole();
                    }
                    catch (Exception ex)
                    {
                        req.Abort(); rep.Close(); rep.Dispose();
                        ex.Message.ErrorLognConsole();
                        SubscribeEventsWork(3000, type, resource);
                    }
                }
            }
            SubscribedEventType = null;
            SubscribedResourceType = null;

        }
        /// <summary>
        /// Stop subscribe event informations.
        /// </summary>
        public void StopSubscribeEvents()
        {            
            eventSubscribeCancellationTokenSource?.Cancel();
            if (eventSubscribeThread?.IsAlive ??false) eventSubscribeThread.Abort();
        }

        #endregion

        #region"log"
        /// <summary>
        /// Get tuner infomations.
        /// <para>GET /log</para>
        /// </summary>
        /// <returns></returns>
        public string GetLogs()
        {
            Uri ub = new Uri(ServiceAddr, "api/log");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return body;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get logs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return null;
            }
        }

        /// <summary>
        /// Show if is log being watched.
        /// </summary>
        public bool IsLogSubscribed => logSubscribeThread?.IsAlive ?? false;
        private Thread logSubscribeThread;
        private CancellationTokenSource logSubscribeCancellationTokenSource;
        /// <summary>
        /// This event will be raised when a log has been recived.
        /// </summary>
        public event LogRecivedHandler LogRecived;
        /// <summary>
        /// Start watch Mirakurun server logs.
        /// GET /log/stream
        /// </summary>
        /// <param name="FailedCallback">Callback delegate in case of failed subscribe.</param>
        public void SubscribeLogs()
        {
            if (logSubscribeThread?.IsAlive ?? false) return;
            var workLoad = new ThreadStart(new Action(() => {
                SubscribeLogsWork();
            }));
            logSubscribeCancellationTokenSource = new CancellationTokenSource();
            logSubscribeThread = new Thread(workLoad);
            logSubscribeThread.IsBackground = true;
            logSubscribeThread.Start();
        }

        private void SubscribeLogsWork(int SleepTime=0)
        {
            Thread.Sleep(SleepTime);
            var ub = new Uri(ServiceAddr, "api/log/stream");
            var req = InitWebRequest(ub);
            req.Timeout = Timeout.Infinite;
            var rep = GettingResponse(ref req);
            if (rep == null) { return; }
            switch ((int)rep.StatusCode)
            {
                case 200:
                    using (StreamReader streadReader = new StreamReader(rep.GetResponseStream(), Encoding.UTF8))
                    {
                        streadReader.BaseStream.ReadTimeout = Timeout.Infinite;
                        //Start parsing cycle
                        try
                        {
                            while (!streadReader.EndOfStream)
                            {
                                logSubscribeCancellationTokenSource.Token.ThrowIfCancellationRequested();
                                string resultLine = streadReader.ReadLine();
                                this.LogRecived?.Invoke(this, resultLine);
                            }
                            req.Abort(); rep.Close(); rep.Dispose();
                            SubscribeLogsWork(3000);
                        }
                        catch (OperationCanceledException)
                        {
                            req.Abort(); rep.Close(); rep.Dispose();
                            "Stoped subsctiption.".InfoLognConsole();
                        }
                        catch (Exception ex)
                        {
                            req.Abort(); rep.Close(); rep.Dispose();
                            ex.Message.ErrorLognConsole();
                            SubscribeLogsWork(3000);
                        }
                    }
                    break;
                default:
                    req.Abort(); rep.Close(); rep.Dispose();
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to subscript logs! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    SubscribeLogsWork(3000);
                    break;
            }
        }

        /// <summary>
        /// Stop watching Mirakurun server Logs.
        /// </summary>
        public void StopSubscribeLogs()
        {
            logSubscribeCancellationTokenSource?.Cancel();
            if (logSubscribeThread?.IsAlive ?? false) logSubscribeThread.Abort();
        }
        #endregion

        #region"misc"
        /// <summary>
        /// Restart mirakurun server.
        /// <para>Mirakurun API:"/restart" Method "PUT"</para>
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public bool RestartService()
        {
            Uri ub = new Uri(ServiceAddr, "api/restart");
            var req = InitWebRequest(ub, RequestMethods.PUT);
            var rep = GettingResponse(ref req);
            switch ((int)rep.StatusCode)
            {
                case 202:
                case 200:
                    req.Abort(); 
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to restart service! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort(); 
                    return false;
            }
        }
        #endregion

        #region"services"
        /// <summary>
        /// Get services information from Mirakurun server.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="serviceId"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="cType"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public IEnumerable<Service> GetServices(int? networkId = null, int? serviceId = null, string name = null, int? type = null, ChannelType? cType = null, long? channel = null)
        {
            var ub = new UriBuilder(ServiceAddr.ToString() + "api/services");
            var query = HttpUtility.ParseQueryString(ub.Query);
            if (serviceId != null) query["serviceId"] = serviceId.ToString();
            if (networkId != null) query["networkId"] = networkId.ToString();
            if (name != null) query["name"] = name;
            if (type != null) query["type"] = type.ToString();
            if (cType != null) query["channel.type"] = cType.ToString();
            if (channel != null) query["channel.channel"] = channel.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString());
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<IEnumerable<Service>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get services! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();

                    return null;
            }
        }
        /// <summary>
        /// Initialize a <see cref="Thread"/> that record certain program's TransportStream from Mirakurun to File.
        /// <para>Mirakurun API: "GET /service/{id}/stream"</para>
        /// </summary>
        /// <param name="serviceId">service Id</param>
        /// <param name="path">Path to the file to record.</param>
        /// <param name="cancelToken">A token to stop record operation can be provide by <see cref="CancellationTokenSource"/>.</param>
        /// <param name="decode"></param>
        /// <param name="priority"></param>
        /// <returns>A <see cref="Thread"/> can be started.</returns>
        public Thread StreamServiceToFile(long serviceId, string path, CancellationToken cancelToken,
                             int? decode = null, StreamPriority priority = StreamPriority.Default)
        {
            var workLoad = new ThreadStart(new Action(() => {
                if (System.IO.File.Exists(path)) return;
                var ub = new UriBuilder(ServiceAddr.ToString() + string.Format("api/services/{0}/stream", serviceId.ToString()));
                var query = HttpUtility.ParseQueryString(ub.Query);
                if (decode != null) query["decode"] = decode.ToString();
                ub.Query = query.ToString();
                var req = InitWebRequest(ub.ToString());
                req.Headers.Add("X-Mirakurun-Priority", ((int)priority).ToString());
                var rep = GettingResponse(ref req);
                if (rep == null) return;
                switch ((int)rep.StatusCode)
                {
                    case 200:
                        using (var writer = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                        {
                            using (var input = rep.GetResponseStream())
                            {
                                byte[] buffer = new byte[188 * 100];
                                int bytesRead;
                                try
                                {
                                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        writer.Write(buffer, 0, bytesRead);
                                        cancelToken.ThrowIfCancellationRequested();
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    Console.WriteLine("Operation canceled!");
                                }
                                catch (Exception e)
                                {
                                    e.Message.ErrorLognConsole();
                                }
                            }
                        }
                        break;
                    //Write to file.
                    case 404:
                        "cannot find chanel. Code:404".ErrorLognConsole();
                        break;
                    case 503:
                        "Tuner Resource Unavailable. Code:503".ErrorLognConsole();
                        break;
                    default:
                        var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                        var json = JsonConvert.DeserializeObject<@default>(mbody);
                        "Failed to get stream! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                        break;
                }
                req.Abort(); rep.Close();
            }));
            return new Thread(workLoad);
        }
        /// <summary>
        /// Get a path can provide stream service from Mirakurun server.
        /// <para>Mirakurun API: "GET /services/{id}/stream"</para>
        /// </summary>
        /// <param name="serviceId">services Id</param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public string GetServiceStreamPath(long serviceId, int? decode = null)
        {
            var ub = new UriBuilder(ServiceAddr.ToString() + string.Format("api/services/{0}/stream", serviceId.ToString()));
            var query = HttpUtility.ParseQueryString(ub.Query);
            if (decode != null) query["decode"] = decode.ToString();
            ub.Query = query.ToString();
            return ub.ToString();
        }

        /// <summary>
        /// Get services infomation by services ID.
        /// <para>Mirakurun API : "/services/{id}"</para>
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>A <see cref="Program"/> object.</returns>
        public Program GetServiceByID(long id)
        {
            Uri ub = new Uri(ServiceAddr, "api/services/" + id.ToString());
            var req = InitWebRequest(ub);
            var rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();

                    return JsonConvert.DeserializeObject<Program>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();

                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get service! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    return null;
            }
        }
        /// <summary>
        /// Get service logo <see cref="Image"/> Object from Mirakurun server.
        /// <para>GET /services/{id}/logo</para>
        /// </summary>
        /// <param name="id">service ID</param>
        /// <returns></returns>
        public Image GetServiceLogoImageByID(long id)
        {//
            Uri ub = new Uri(ServiceAddr, "api/services/" + id.ToString()+ "/logo");
            var req = InitWebRequest(ub);
            var rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    using (var st = rep.GetResponseStream())
                    {
                        return Image.FromStream(st);
                    }
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();

                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get Logo! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    return null;
            }
        }
        /// <summary>
        /// Get service logo data in byte array from Mirakurun server.
        /// <para>GET /services/{id}/logo</para>
        /// </summary>
        /// <param name="id">service ID</param>
        /// <returns></returns>
        public byte[] GetServiceLogoRawByID(long id)
        {
            Uri ub = new Uri(ServiceAddr, "api/services/" + id.ToString()+ "/logo");
            var req = InitWebRequest(ub);
            var rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    using (var st = rep.GetResponseStream())
                    {
                        byte[] buffer = new byte[rep.ContentLength];
                        st.ReadAsync(buffer, 0, buffer.Length);
                        return buffer;
                    }
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();

                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get Logo! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    return null;
            }
        }
        #endregion

        #region"status"
        /// <summary>
        /// Get Mirakurun service status.
        /// <para>GET /status</para>
        /// </summary>
        /// <returns></returns>
        public Status GetStatus()
        {
            Uri ub = new Uri(ServiceAddr, "api/status/");
            var req = InitWebRequest(ub);
            var rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<Status>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();

                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get status! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    return null;
            }
        }
        #endregion

        #region"tuners"
        /// <summary>
        /// Get tuner infomations.
        /// <para>GET /tuners</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuner> GetTuners()
        {
            Uri ub = new Uri(ServiceAddr, "api/tuners");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<IEnumerable<Tuner>>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get tuners! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get tuner's working process PID by tuner ID.
        /// <para>GET /tuners/{index}/process</para>
        /// </summary>
        /// <param name="tunerId"></param>
        /// <returns></returns>
        public int GetTunerProcessPID(int tunerId)
        {//GET /tuners/{index}/process
            Uri ub = new Uri(ServiceAddr, "api/tuners/"+tunerId.ToString()+"/process");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return 0;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    var pObj = JObject.Parse(body);
                    return pObj.GetValue("pid").ToObject<int>();
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get process id! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return 0;
            }
        }
        /// <summary>
        /// Kill tuner's working process by tuner ID.
        /// <para>DELETE /tuners/{index}/process</para>
        /// </summary>
        /// <param name="tunerId"></param>
        /// <returns></returns>
        public bool KillTunerProcessByID(int tunerId)
        {//GET /tuners/{index}/process
            Uri ub = new Uri(ServiceAddr, "api/tuners/" + tunerId.ToString() + "/process");
            var req = InitWebRequest(ub,RequestMethods.DELETE);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    req.Abort();
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to kill process! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return false;
            }
        }
        /// <summary>
        /// Get tuner infomations.
        /// <para>GET /tuners/{index}</para>
        /// </summary>
        /// <returns></returns>
        public Tuner GetTunerById(int tunerId)
        {
            Uri ub = new Uri(ServiceAddr, "api/tuners/"+tunerId.ToString());
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<Tuner>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get tuner info! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return null;
            }
        }
        #endregion

        #region"version"
        /// <summary>
        /// Update server, this may not working as good as using npm to update server.
        /// <para>PUT /version/update</para>
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public bool ServerUpdate(bool force = false)
        {
            var ub = new UriBuilder(ServiceAddr.ToString() + "api/version/update");
            var query = HttpUtility.ParseQueryString(ub.Query);
            query["force"] = force.ToString();
            ub.Query = query.ToString();
            var req = InitWebRequest(ub.ToString(), RequestMethods.PUT);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return false;
            switch ((int)rep.StatusCode)
            {
                case 202:
                case 200:
                    req.Abort();
                    return true;
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to start server update! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return false;
            }
        }
        /// <summary>
        /// Get server version infomations.
        /// <para>GET /version</para>
        /// </summary>
        /// <returns></returns>
        public ServerVersion GetServerVersion()
        {
            Uri ub = new Uri(ServiceAddr, "/api/version");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) return null;
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<ServerVersion>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get version! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    req.Abort();
                    return null;
            }
        }
        /// <summary>
        /// Get server version infomations.
        /// <para>GET /version</para>
        /// </summary>
        /// <returns></returns>
        private ServerVersion GetServerVersion(out Exception e)
        {
            e = null;
            Uri ub = new Uri(ServiceAddr, "/api/version");
            var req = InitWebRequest(ub);
            HttpWebResponse rep = GettingResponse(ref req);
            if (rep == null) {
                e = new WebException("Unable to reach server.",WebExceptionStatus.SendFailure);
                return null;
            }
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var body = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    req.Abort();
                    return JsonConvert.DeserializeObject<ServerVersion>(body);
                default:
                    var mbody = GetResponseBodyString(ref rep, Encoding.UTF8, true);
                    var json = JsonConvert.DeserializeObject<@default>(mbody);
                    "Failed to get version! [{0}] reason: {1}".ErrorLognConsole(json.code, json.reason);
                    e = new WebException(string.Format("[{0}]{1}",json.code,json.reason),WebExceptionStatus.Success);
                    req.Abort();
                    return null;
            }
        }
        #endregion

        #region IDisposable Support
        /// <summary>
        /// Show this object is Disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; } = false;
        /// <summary>
        /// Release resource in this object.
        /// </summary>
        public void Dispose()
        {
            //if (this.IsDisposed) return; //Redo dispose make no hurts, let them do it.
            if (eventSubscribeThread?.IsAlive ?? false) eventSubscribeThread.Abort();
            eventSubscribeCancellationTokenSource?.Dispose();
            if (logSubscribeThread?.IsAlive ?? false) logSubscribeThread.Abort();
            logSubscribeCancellationTokenSource?.Dispose();
            IsDisposed = true;
        }
        #endregion
    }
}
