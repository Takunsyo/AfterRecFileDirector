using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
