using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.File;
using ZhonTai.Admin.Domain.OrderDemand;

namespace ZhonTai.Admin.Domain.SalesTarget
{
    /// <summary>
    /// 销售指标
    /// </summary>
    [Table(Name = "ad_sales_target")]
    public class SalesTargetEntity : EntityBase
    {
        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 所属年月yyyy-MM
        /// </summary>
        public string YearMonth { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SalesTargetStatus Status { get; set; }

        /// <summary>
        /// 附件Id
        /// </summary>
        public long? FileId { get; set; }

        /// <summary>
        /// 是否存在警告
        /// </summary>
        public bool HasWarning { get; set; }

        /// <summary>
        /// 销售指标子项列表
        /// </summary>
        [Navigate(nameof(SalesTargetItemEntity.SalesTargetId))]
        public List<SalesTargetItemEntity> Items { get; set; }


        [Navigate(nameof(FileId))]
        public FileEntity File { get; set; }
    }
}
