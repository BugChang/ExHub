using System.Collections.Generic;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.User;

namespace ZhonTai.Admin.Domain.Dealer
{
    /// <summary>
    /// 经销商
    /// </summary>
    [Table(Name = "ad_dealer")]
    public class DealerEntity : EntityBase
    {
        /// <summary>
        /// SoldToCode
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 经销商类别
        /// </summary>
        public DealerType Type { get; set; }

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
        public long BizUserId { get; set; }

        /// <summary>
        /// ExHub负责人
        /// </summary>
        public long ExHubUserId { get; set; }

        /// <summary>
        /// 大区经理
        /// </summary>
        public long RegionManagerUserId { get; set; }

        /// <summary>
        /// 经销商状态
        /// </summary>
        public DealerStatus Status { get; set; }

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
        /// 证书列表
        /// </summary>
        [Navigate(nameof(DealerLicenseEntity.SoldCode), TempPrimary = nameof(SoldCode))]
        public List<DealerLicenseEntity> Licenses { get; set; }

        /// <summary>
        /// 地址列表
        /// </summary>
        [Navigate(nameof(DealerAddressEntity.SoldCode), TempPrimary = nameof(SoldCode))]
        public List<DealerAddressEntity> Addresses { get; set; }

        [Navigate(nameof(RegionManagerUserId))]
        public UserEntity RegionManager { get; set; }

        [Navigate(nameof(BizUserId))]
        public UserEntity BizUser { get; set; }

        [Navigate(nameof(ExHubUserId))]
        public UserEntity ExHubUser { get; set; }
    }
}
