using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.Contract;
using ZhonTai.Admin.Domain.Express;

namespace ZhonTai.Admin.Domain.Order
{
    /// <summary>
    /// 订单
    /// </summary>
    [Table(Name = "ad_order")]
    public class OrderEntity : EntityBase
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
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 存在警告
        /// </summary>
        public bool HasWarning { get; set; }

        /// <summary>
        /// SAP 创建时间
        /// </summary>
        public DateTime? SapCreatedTime { get; set; }

        /// <summary>
        /// SAP 创建人
        /// </summary>
        public string SapCreatedBy { get; set; }

        /// <summary>
        /// 发运批号
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 物流锁定
        /// </summary>
        public string DeliveryBlock { get; set; }


        /// <summary>
        /// 订单项列表
        /// </summary>
        [Navigate(nameof(OrderItemEntity.OrderId))]
        public List<OrderItemEntity> Items { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        [Navigate(nameof(SoCode), TempPrimary = nameof(ContractEntity.SoCode))]
        public ContractEntity Contract { get; set; }

        /// <summary>
        /// 发票
        /// </summary>
        [Navigate(nameof(SoCode), TempPrimary = nameof(InvoiceEntity.SoCode))]
        public List<InvoiceEntity> Invoices { get; set; }

        /// <summary>
        /// 物流信息
        /// </summary>
        [Navigate(nameof(SoCode), TempPrimary = nameof(ExpressEntity.SoCode))]
        public ExpressEntity Express { get; set; }

    }
}
