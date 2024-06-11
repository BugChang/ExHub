using System;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Express
{    
    /// <summary>
     /// 物流
     /// </summary>
    [Table(Name = "ad_express_batch")]
    public class ExpressBatchEntity : EntityBase
    {

        /// <summary>
        /// 物流ID
        /// </summary>
        public long ExpressId { get; set; }

        /// <summary>
        /// 发运批号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ValidityPeriod { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string Count { get; set; }
    }
}
