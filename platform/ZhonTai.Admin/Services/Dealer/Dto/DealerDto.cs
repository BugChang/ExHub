using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Dealer.Dto
{
    public class DealerDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// SoldToCode
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// CSL商务
        /// </summary>
        public string BizUserName { get; set; }

        /// <summary>
        /// ExHub
        /// </summary>
        public string ExHubUserName { get; set; }

        /// <summary>
        /// 大区经理
        /// </summary>
        public string RegionManagerUserName { get; set; }

        /// <summary>
        /// 授权代表名称
        /// </summary>
        public string DelegatorRealName { get; set; }

        /// <summary>
        /// 授权代表账号
        /// </summary>
        public string DelegatorUserName { get; set; }

    }
}
