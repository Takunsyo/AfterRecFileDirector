using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RVMCore.TelgeramBot.Apis.Objects;
using RVMCore.TelgeramBot.Apis;
using System.Net.Http;

namespace RVMCore.TelgeramBot
{
    public class Bot
    {
        public string Token { get; private set; } = "685379411:AAFdW8jh1t8uhr5N7leYM8pW-ldtZLL807Y";

        private const string ServerAddr = "https://api.telegram.org/";

        public Bot(string mToken)
        {
            Token = mToken;
        }

        #region private methods
        /// <summary>
        /// Init a POST <see cref="HttpWebRequest"/> to <see cref="Uri"/>.
        /// </summary>
        private HttpWebRequest InitHttpWebRequest(Uri uri)
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(uri);
            myRequest.Method = "POST";
            myRequest.Timeout = Timeout.Infinite;
            return myRequest;
        }
        /// <summary>
        /// Init a POST <see cref="HttpWebRequest"/> to <see cref="string"/> uri.
        /// </summary>
        private HttpWebRequest InitHttpWebRequest(string uri)
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(uri);
            myRequest.Method = "POST";
            myRequest.Timeout = Timeout.Infinite;
            return myRequest;
        }
        /// <summary>
        /// Use a <see cref="HttpWebRequest"/> to get a <see cref="HttpWebResponse"/>.
        /// <para>If error happens a <see cref="Exception"/> will pass back to parameter e.</para>
        /// </summary>
        private HttpWebResponse GetHttpWebRespond(HttpWebRequest request, out Exception e)
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
        /// <summary>
        /// Use a <see cref="HttpWebRequest"/> to get a <see cref="HttpWebResponse"/>.
        /// </summary>
        private HttpWebResponse GetHttpWebRespond(HttpWebRequest request)
        {
            Exception e;
            return GetHttpWebRespond(request, out e);
        }
        /// <summary>
        /// Send a POST request to <see cref="Uri"/> then get a <see cref="HttpWebResponse"/> from the request.
        /// <para>If error happens a <see cref="Exception"/> will pass back to parameter e.</para>
        /// </summary>
        private HttpWebResponse GetHttpWebRespond(Uri uri, out Exception e)
        {
            return GetHttpWebRespond(InitHttpWebRequest(uri), out e);
        }
        /// <summary>
        /// Send a POST request to <see cref="Uri"/> then get a <see cref="HttpWebResponse"/> from the request.
        /// </summary>
        private HttpWebResponse GetHttpWebRespond(Uri uri)
        {
            return GetHttpWebRespond(InitHttpWebRequest(uri));
        }
        /// <summary>
        /// Send a POST request to <see cref="string"/> uri then get a <see cref="HttpWebResponse"/> from the request.
        /// <para>If error happens a <see cref="Exception"/> will pass back to parameter e.</para>
        /// </summary>
        private HttpWebResponse GetHttpWebRespond(string uri, out Exception e)
        {
            return GetHttpWebRespond(InitHttpWebRequest(uri), out e);
        }
        /// <summary>
        /// Send a POST request to <see cref="string"/> uri then get a <see cref="HttpWebResponse"/> from the request.
        /// </summary>
        private HttpWebResponse GetHttpWebRespond(string uri)
        {
            return GetHttpWebRespond(InitHttpWebRequest(uri));
        }

        private static string GetResponseBodyString(ref HttpWebResponse response, bool CloseAfterWork = true)
        {
            if (response == null || response.ContentLength <= 0) return null;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                var tmp = reader.ReadToEnd();
                if (CloseAfterWork) response.Close();
                return tmp;
            }
        }
        #endregion

        public void GetUpdates()
        {
            string uri = string.Format("{0}bot{1}/getUpdates", ServerAddr, Token);
            uri = uri + "?timeout=600";
            var req = InitHttpWebRequest(uri);
            var rep = GetHttpWebRespond(req);
            //using (var client = new HttpClient())
            //{
            //    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

            //    var request = new HttpRequestMessage(HttpMethod.Get, uri);
            //    using (var response = await client.SendAsync(
            //        request,
            //        HttpCompletionOption.ResponseHeadersRead))
            //    {
            //        using (var body = await response.Content.ReadAsStreamAsync())
            //        using (var reader = new StreamReader(body))
            //            while (!reader.EndOfStream)
            //                Console.WriteLine(reader.ReadLine());
            //    }
            //}
            using (var streadReader = new StreamReader(rep.GetResponseStream()))
            {
                streadReader.BaseStream.ReadTimeout = Timeout.Infinite;
                try
                {
                    while (!streadReader.EndOfStream)
                    {
                        string resultLine = streadReader.ReadLine();
                        //System.Diagnostics.Trace.WriteLine(resultLine);
                        if (!resultLine.StartsWith("{")) continue;
                        if (!resultLine.EndsWith("}")) continue;
                        var tmp = JsonConvert.DeserializeObject<Update>(resultLine);
                        Console.WriteLine(tmp.message.text);
                    }
                }
                catch (OperationCanceledException)
                {
                    "Stoped subsctiption.".InfoLognConsole();
                }
                Console.WriteLine("Session out.");
            }
        }

        public User GetMe()
        {
            string uri = string.Format("{0}bot{1}/getMe", ServerAddr, Token);
            var rep = GetHttpWebRespond(uri);
            var json = GetResponseBodyString(ref rep);
            switch ((int)rep.StatusCode)
            {
                case 200:
                    var tmp = JsonConvert.DeserializeObject<ApiBase<User>>(json);
                    return tmp.result;
                default:
                    Console.WriteLine("Error");
                    return null;
            }
        }
    }
}
