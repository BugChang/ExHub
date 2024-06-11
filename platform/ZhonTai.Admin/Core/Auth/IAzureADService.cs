using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Services.Auth.Dto;

namespace ZhonTai.Admin.Core.Auth
{
    public interface IAzureAdService
    {
        OidcUserinfoView GetOidcUserinfoAsync(string accessToken);

        string GetAuthorizeUserInfoUrl();
    }
}
