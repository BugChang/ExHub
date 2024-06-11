using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Common.Extensions;

namespace ZhonTai.Admin.Services.Dealer.Dto
{

    public class DealerGetListOutput
    {
        /// <summary>
        /// 经销商Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// SoldCode
        /// </summary>
        public string SoldCode { get; set; }
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
        /// ExHub负责人
        /// </summary>
        public string ExHubUserRealName { get; set; }
        /// <summary>
        /// CSL商务
        /// </summary>
        public string BizUserRealName { get; set; }

        /// <summary>
        /// 大区经理
        /// </summary>
        public string RegionManagerRealName { get; set; }

        /// <summary>
        /// 名称代码只读字段
        /// </summary>
        public string NameMergeText => $"{Name}/{SoldCode}";

        /// <summary>
        /// 经销商类型
        /// </summary>
        public DealerType DealerType { get; set; }

        /// <summary>
        /// 经销商类型描述
        /// </summary>
        public string DealerTypeDesc  => DealerType.ToDescription();

        /// <summary>
        /// 经销商状态
        /// </summary>
        public DealerStatus DealerStatus { get; set; }

        /// <summary>
        /// 经销商状态描述
        /// </summary>
        public string DealerStatusDesc => DealerStatus.ToDescription();

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 收件人联系方式
        /// </summary>
        public string ReceiverMobile { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedUserName { get; set; }


        /// <summary>
        /// 更新人真实姓名
        /// </summary>
        public string ModifiedUserRealName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

    }
}
