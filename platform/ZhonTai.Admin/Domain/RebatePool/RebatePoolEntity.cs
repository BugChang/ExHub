using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.RebatePool
{
    /// <summary>
    /// 返利池
    /// </summary>
    [Table(Name = "ad_rebate_pool")]
    public class RebatePoolEntity : EntityBase
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
        /// 返利池金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 返利更新时间
        /// </summary>
        public DateTime? RebateUpdateTime { get; set; }
    }
}
