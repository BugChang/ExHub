using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.OrderDemand
{
    /// <summary>
    /// 订单需求项
    /// </summary>
    [Table(Name = "ad_order_demand_item")]
    public class OrderDemandItemEntity : EntityBase
    {
        /// <summary>
        /// 订单需求Id
        /// </summary>
        public long OrderDemandId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// 产品规格
        /// </summary>
        [MaxLength(200)]
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(50)]
        public string Unit { get; set; }

        /// <summary>
        /// 需求量
        /// </summary>
        public int NeedCount { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 剩余配额
        /// </summary>
        public int RemainingQuota { get; set; }

        /// <summary>
        /// 返利金额 
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// 接受零头箱
        /// </summary>
        public bool AcceptPartialBox { get; set; }

        /// <summary>
        /// 是否使用折扣
        /// </summary>
        public bool IsUseDiscount { get; set; }

        /// <summary>
        /// 箱规
        /// </summary>
        public int BoxSize { get; set; }
    }
}
