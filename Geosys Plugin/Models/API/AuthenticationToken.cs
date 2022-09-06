using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Models.APIResponses
{
    public class AuthenticationTokenInput
    {

        public string client_id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string scope { get; set; }

        public string client_secret { get; set; }

        public string grant_type { get; set; }

        public AuthenticationTokenInput(string client_id, string username, string password, string client_secret)
        {
            this.client_id = client_id;
            this.username = username;
            this.password = password;
            this.client_secret = client_secret;
            this.scope = "openid offline_access";
            this.grant_type = "password";
        }

    }

    public class AuthenticationTokenResponse
    {

        public string access_token { get; set; }

        public int expires_in { get; set; }

        public string token_type { get; set; }

        public string scope { get; set; }

    }


}
