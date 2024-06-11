using FreeSql.DataAnnotations;
using System;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.Product;

namespace ZhonTai.Admin.Domain.Quota
{
    /// <summary>
    /// 配额
    /// </summary>
    [Table(Name = "ad_quota")]
    public class QuotaEntity : EntityBase
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }


        /// <summary>
        /// 配额总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 配额总数调整量
        /// </summary>
        public int TotalChange { get; set; }

        /// <summary>
        /// 获批数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获批数量（最新）
        /// </summary>
        public int ApprovedCount { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        public string YearMonth { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public QuotaStatus Status { get; set; }

        /// <summary>
        /// 审批Id
        /// </summary>
        public long ApprovedId { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }


        [Navigate(nameof(ProductCode), TempPrimary = nameof(Product.Code))]
        public ProductEntity Product { get; set; }

    }
}
