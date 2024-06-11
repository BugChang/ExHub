using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.SalesTarget
{

    /// <summary>
    /// 销售指标项
    /// </summary>
    [Table(Name = "ad_sales_target_item")]
    public class SalesTargetItemEntity : EntityBase
    {
        /// <summary>
        /// 销售指标Id
        /// </summary>
        public long SalesTargetId { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }
}
