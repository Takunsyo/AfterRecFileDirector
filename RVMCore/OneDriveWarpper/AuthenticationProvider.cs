using Microsoft.Graph;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RVMCore.OneDriveWarpper
{
    class AuthenticationProvider : IAuthenticationProvider
    {

        string tokenType;
        string accessToken;
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(this.tokenType, this.accessToken);
                
        }
    }
}
