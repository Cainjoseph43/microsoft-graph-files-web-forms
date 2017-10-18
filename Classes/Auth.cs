using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace microsoft_graph_files_web_forms.Classes
{
    public class Auth
    {
        // Helper classes
        public class TokenResponse
        {
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string ext_expires_in { get; set; }
            public string access_token { get; set; }
        }

        // Attributes
        public string AppAccessToken = "";
        private string URL = System.Web.Configuration.WebConfigurationManager.AppSettings["MSOauthURL"];
        private string CLIENT_ID = System.Web.Configuration.WebConfigurationManager.AppSettings["CLIENT_ID"];
        private string APP_SECRET = System.Web.Configuration.WebConfigurationManager.AppSettings["APP_SECRET"];

        // Constructor
        public Auth()
        {

        }

        // Behaviors
        public bool GetAppAccessToken()
        {
            bool success = false;
            string result = "";
            var body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", CLIENT_ID),
                new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
                new KeyValuePair<string, string>("client_secret", APP_SECRET),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(URL);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            System.Net.Http.HttpContent content = new FormUrlEncodedContent(body);
            HttpResponseMessage res = client.PostAsync(URL, content).Result;

            result = res.Content.ReadAsStringAsync().Result;

            if (res.IsSuccessStatusCode)
            {
                TokenResponse t = JsonConvert.DeserializeObject<TokenResponse>(result);
                AppAccessToken = t.access_token;
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }
    }
}