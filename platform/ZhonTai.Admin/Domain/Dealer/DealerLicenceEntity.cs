using FreeSql.DataAnnotations;
using System;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Dealer
{
    /// <summary>
    /// 经销商证照
    /// </summary>
    [Table(Name = "ad_dealer_license")]
    public class DealerLicenseEntity : EntityBase
    {
        /// <summary>
        /// 客户编码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 证照名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        [Navigate(nameof(SoldCode), TempPrimary = nameof(Dealer.SoldCode))]
        public DealerEntity Dealer { get; set; }
    }
}
