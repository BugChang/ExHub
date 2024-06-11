using System.Collections.Generic;
using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.Order;
using System.ComponentModel;
using System;

namespace ZhonTai.Admin.Domain.OrderDemand
{
    /// <summary>
    /// 订单需求
    /// </summary>
    [Table(Name = "ad_order_demand")]
    public class OrderDemandEntity : EntityBase
    {
        /// <summary>
        /// PR Code
        /// </summary>
        [MaxLength(20)]
        public string PrCode { get; set; }
        /// <summary>
        /// 经销商代码
        /// </summary>
        [MaxLength(50)]
        public string SoldCode { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        [MaxLength(100)]
        public string DealerName { get; set; }

        /// <summary>
        /// 地址代码
        /// </summary>
        [MaxLength(20)]
        public string ShipCode { get; set; }

        /// <summary>
        /// 经销商地址
        /// </summary>
        [MaxLength(200)]
        public string DealerAddress { get; set; }

        /// <summary>
        /// 是否加急
        /// </summary>
        public bool IsUrgent { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderDemandStatus Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Remark { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        [MaxLength(20)]
        public string Region { get; set; }


        /// <summary>
        /// 首次提交时间
        /// </summary>
        public DateTime? FirstCommitTime { get; set; }

        /// <summary>
        /// 订单需求项
        /// </summary>
        [Navigate(nameof(OrderDemandItemEntity.OrderDemandId))]
        public List<OrderDemandItemEntity> Items { get; set; } = new();

        [Navigate(nameof(OrderEntity.PrCode), TempPrimary = nameof(PrCode))]
        public List<OrderEntity> Orders { get; set; }

    }
}
