using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ZhonTai.Admin.Services.Order.Dto
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public class OrderGetOutput : OrderDto
    {
        /// <summary>
        /// 订单项
        /// </summary>
        public List<OrderItemDto> Items { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalAmount => Items.Sum(a => a.TotalAmount);

        /// <summary>
        /// 合同信息
        /// </summary>
        public OrderContractDto Contract { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        public List<OrderInvoiceDto> Invoices { get; set; }

        /// <summary>
        /// 物流信息
        /// </summary>
        public OrderExpressDto Express { get; set; }

        /// <summary>
        /// 物流批次
        /// </summary>
        public List<OrderExpressBatchDto> ExpressBatches { get; set; }

    }

    /// <summary>
    /// 发票信息
    /// </summary>
    public class OrderInvoiceDto
    {
        /// <summary>
        /// PDF URL
        /// </summary>
        public string PdfUrl { get; set; }

        /// <summary>
        /// XML Url
        /// </summary>
        public string XmlUrl { get; set; }

        /// <summary>
        /// Ofd Url
        /// </summary>
        public string OfdUrl { get; set; }

        /// <summary>
        /// GTS发票号
        /// </summary>
        public string GtsNo { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime? InvoiceTime { get; set; }
    }

    /// <summary>
    /// 合同信息
    /// </summary>
    public class OrderContractDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ContractId { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

    }

    /// <summary>
    /// 物流信息
    /// </summary>
    public class OrderExpressDto
    {

        /// <summary>
        /// 单据编号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 预计到货时间
        /// </summary>
        public DateTime? ExpectedArrivalTime { get; set; }

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? ReceivedTime { get; set; }

        /// <summary>
        /// 司机信息
        /// </summary>
        public string DriverInfo { get; set; }

        /// <summary>
        /// 发货仓
        /// </summary>
        public string DeliveryWarehouse { get; set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        public string TransportMethod { get; set; }

        /// <summary>
        /// 运输方式描述
        /// </summary>
        public string TransportMethodDesc
        {
            get
            {
                switch (TransportMethod)
                {
                    case "02":
                        return "公路";
                    case "04":
                        return "快递";
                    case "05":
                        return "铁路";
                    case "06":
                        return "空运";
                    case "07":
                        return "海运";
                    default:
                        return TransportMethod;
                }
            }
        }

        /// <summary>
        /// 承运单位
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 释放时间
        /// </summary>
        public DateTime? ReleasedTime { get; set; }

        /// <summary>
        /// PGI 时间
        /// </summary>
        public DateTime? PgiTime { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public string Status => GetExpress(ReleasedTime, PgiTime, ReceivedTime);

        private string GetExpress(DateTime? releasedTime, DateTime? pgiTime, DateTime? receivedTime)
        {
            if (receivedTime.HasValue)
            {
                return ExpressStatus.Signed.ToDescriptionOrString();
            }
            else if (pgiTime.HasValue)
            {
                return ExpressStatus.Shipped.ToDescriptionOrString();
            }
            else if (releasedTime.HasValue)
            {
                return ExpressStatus.Released.ToDescriptionOrString();
            }
            throw new ArgumentNullException($"未知物流状态");
        }
    }

    /// <summary>
    /// 物流批次信息
    /// </summary>
    public class OrderExpressBatchDto
    {
        /// <summary>
        /// 发运批号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public string ValidityPeriod { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string Count { get; set; }
    }

    public enum ExpressStatus
    {
        [Description("已释放")]
        Released = 0,
        [Description("已发运")]
        Shipped = 1,
        [Description("已签收")]
        Signed = 2
    }



}
