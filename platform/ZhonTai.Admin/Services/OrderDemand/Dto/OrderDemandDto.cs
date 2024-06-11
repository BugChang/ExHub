using System;
using ZhonTai.Admin.Domain.OrderDemand;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandDto
    {
        /// <summary>
        /// 订单需求ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 经销商地址代码
        /// </summary>
        public string ShipCode { get; set; }

        /// <summary>
        /// 是否加急
        /// </summary>
        public bool IsUrgent { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// PR Code
        /// </summary>
        public string PrCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string DealerAddress { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderDemandStatus Status { get; set; }

    }
}
