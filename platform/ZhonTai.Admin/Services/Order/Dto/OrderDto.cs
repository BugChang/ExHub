using System;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Services.Order.Dto
{
    /// <summary>
    /// 订单关联
    /// </summary>
    public class OrderDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 订单代码
        /// </summary>
        public string SoCode { get; set; }

        /// <summary>
        /// 引用代码（匹配PrCode）
        /// </summary>
        public string ReferenceCode { get; set; }

        /// <summary>
        /// 订单需求代码
        /// </summary>
        public string PrCode { get; set; }
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 经销商收货地址代码
        /// </summary>
        public string ShipCode { get; set; }

        /// <summary>
        /// 经销商收货地址
        /// </summary>
        public string DealerAddress { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 订单锁定
        /// </summary>
        public DateTime? SapCreatedTime { get; set; }

        /// <summary>
        /// SAP 创建人
        /// </summary>
        public string SapCreatedBy { get; set; }

        /// <summary>
        /// 物流锁定
        /// </summary>
        public string DeliveryBlock { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

 

    }
}
