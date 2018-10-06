using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RVMCore.Google
{
    class GoogleDrive
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "After Record File Upload Service";
        static UserCredential UserCredential {
            get
            {
                return Credential.GetUserCredential();
            }
        }

        public DriveService Service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = UserCredential,
            ApplicationName = ApplicationName,
        });

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
                return string.Format("{{\"installed\":{0}}}",JsonConvert.SerializeObject(new Credential()));
            }

            public static UserCredential GetUserCredential()
            {
                string credPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RVMGoogle", "4vSX3zzWyv7HgY-PrORjG_5I.json");
                using (var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(Credential.GetClientString())))
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
    }
    
}
