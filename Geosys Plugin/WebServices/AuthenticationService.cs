using Geosys_Plugin.Models;
using Geosys_Plugin.Models.APIResponses;
using Geosys_Plugin.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.WebServices
{
    public class AuthenticationService : WebServiceBase
    {
        AuthenticationUrls _authenticationUrls;

        public AuthenticationService()
        {
            _authenticationUrls = new APIUrls().authentication;
        }

        public async Task<AuthenticationTokenResponse> GetAuthenticationToken(AuthenticationTokenInput input)
        {
            var json = JsonConvert.SerializeObject(input);
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string url = null;
            if (Properties.Settings.Default.UseTestingService)
            {
                url = string.Format(@"{0}/{1}", _authenticationUrls.BaseTestUrl, _authenticationUrls.GetToken);
            }
            else
            {
                url = string.Format(@"{0}/{1}", _authenticationUrls.BaseUrl, _authenticationUrls.GetToken);
            }

            return await PostRequest<AuthenticationTokenResponse>(url, parameters);
        }

        public async Task RenewAuthenticationTokenIfNeeded(bool useTestingService = false)
        {
            string token = Properties.Settings.Default.AuthToken;
            bool isValid = !TokenUtils.IsEmptyOrInvalid(token);
            if (!isValid)
            {
                string _clientId = Properties.Settings.Default.ClientId;
                string _username = Properties.Settings.Default.Username;
                string _password = Properties.Settings.Default.Password;
                string _clientSecret = Properties.Settings.Default.ClientSecret;
                AuthenticationTokenInput input = new AuthenticationTokenInput(_clientId, _username, _password, _clientSecret);
                AuthenticationTokenResponse newToken = await this.GetAuthenticationToken(input);
                if (newToken != null && newToken.access_token != null)
                {
                    Properties.Settings.Default.AuthToken = newToken.access_token;
                }
                else
                {
                    Properties.Settings.Default.AuthToken = null;
                }
                Properties.Settings.Default.Save();
            }
        }
    }
}
