using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Auth.Dto
{
    public class OidcUserinfoView
    {
        /// <summary>
        /// 
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string family_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string given_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string email { get; set; }
    }
}
