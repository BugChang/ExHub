using System;

namespace ZhonTai.Admin.Services.Order.Dto
{
    /// <summary>
    /// 订单导入
    /// </summary>
    public class OrderImportDto
    {
        /// <summary>
        /// 订单代码
        /// </summary>
        public string SoCode { get; set; }

        /// <summary>
        /// 引用代码（匹配PrCode）
        /// </summary>
        public string ReferenceCode { get; set; }

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
        public int? Count { get; set; }

        /// <summary>
        /// SAP 创建时间
        /// </summary>
        public DateTime? SapCreatedTime { get; set; }

        /// <summary>
        /// 物流锁定
        /// </summary>
        public string DeliveryBlock { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// 订单取消原因
        /// </summary>
        public string CancelReason { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string OrderType { get; set; }


        /// <summary>
        /// SAP创建人
        /// </summary>
        public string SapCreatedBy { get; set; }


        /// <summary>
        /// 发运批号
        /// </summary>
        public string Batch { get; set; }

    }
}
