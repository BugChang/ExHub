using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Newtonsoft.Json;
using NPOI.POIFS.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZhonTai.Admin.Services.Auth.Dto;

namespace ZhonTai.Admin.Core.Auth
{
    public class AzureAdService : IAzureAdService
    {
        private string ClientId { get; set; }
        private string TenantId { get; set; }
        private string RedirectUrl { get; set; }
        private string AuthorizeUrl { get; set; }
        private string UserinfoUrl { get; set; }

        public AzureAdService(IConfiguration configuration)
        {
            ClientId = configuration.GetValue<string>("AzureAD:ClientId");
            TenantId = configuration.GetValue<string>("AzureAD:TenantId"); ;
            RedirectUrl = configuration.GetValue<string>("AzureAD:RedirectUrl");

            AuthorizeUrl = "https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize";
            UserinfoUrl = "https://graph.microsoft.com/oidc/userinfo";

        }

        public AzureAdService(string clientId, string tenantId, string redirectUrl,
            string authorizeUrl, string userinfoUrl)
        {
            this.ClientId = clientId;
            this.TenantId = tenantId;
            this.RedirectUrl = redirectUrl;
            this.AuthorizeUrl = authorizeUrl;
            this.UserinfoUrl = userinfoUrl;
        }

        /// <summary>
        /// (单点登录) 获取用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public OidcUserinfoView GetOidcUserinfoAsync(string accessToken)
        {
            HttpClient webClient = null;
            webClient = new HttpClient();
            webClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            webClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var oidcUserinfoResult = webClient.GetStringAsync(UserinfoUrl).Result;
            if (string.IsNullOrWhiteSpace(oidcUserinfoResult))
            {
                return null;
            }

            var oidcUserinfo = JsonConvert.DeserializeObject<OidcUserinfoView>(oidcUserinfoResult);

            if (string.IsNullOrEmpty(oidcUserinfo.email))
            {
                string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
                GraphServiceClient graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    return Task.FromResult(0);
                }));

                var user = graphClient.Me.Request().GetAsync().Result;
                string userPrincipalName = user.UserPrincipalName;
                oidcUserinfo.email = userPrincipalName;
            }

            return oidcUserinfo;
        }

        /// <summary>
        /// (单点登录)获取登录请求url(获取用于调用 UserInfo 的访问令牌)
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizeUserInfoUrl()
        {
            var apiUrl = AuthorizeUrl.Replace("{tenant}", TenantId);

            StringBuilder urlParam = new StringBuilder();
            urlParam.Append($"?client_id={ClientId}");
            urlParam.Append($"&response_type=id_token%20token");
            if (!string.IsNullOrWhiteSpace(RedirectUrl))
            {
                urlParam.Append($"&redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}");
            }
            urlParam.Append($"&response_mode=fragment");//form_get  form_post
            urlParam.Append($"&scope=user.read+Offline_Access+openid+profile+email");
            urlParam.Append($"&nonce={Guid.NewGuid().ToString("N")}");
            urlParam.Append($"&state={Guid.NewGuid().ToString("N")}");

            apiUrl += urlParam.ToString();

            return apiUrl;
        }
    }

}
