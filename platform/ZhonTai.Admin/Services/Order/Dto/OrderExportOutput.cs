using Npoi.Mapper.Attributes;

namespace ZhonTai.Admin.Services.Order.Dto
{
    // 导出订单
    public class OrderExportOutput
    {
        [Column("PR Code")]
        public string PrCode { get; set; }
        [Column("SO Code")]
        public string SoCode { get; set; }
        [Column("经销商名称")]
        public string DealerName { get; set; }
        [Column("数量")]
        public int OrderCount { get; set; }
        [Column("订单状态")]
        public string Status { get; set; }
        [Column("SAP创建时间")]
        public string SapCreatedTime { get; set; }
        [Column("区域")]
        public string Region { get; set; }
        [Column("创建人")]
        public string SapCreatedUser { get; set; }
        [Column("Sold-to")]
        public string SoldCode { get; set; }
        [Column("Ship-to")]
        public string ShipCode { get; set; }
        [Column("Ship-to地址")]
        public string DealerAddress { get; set; }
        [Column("Reference Code")]
        public string ReferenceCode { get; set; }

        [Column("Delivery Block")]
        public string DeliveryBlock { get; set; }

        [Column("折扣后金额")] 
        public decimal Amount { get; set; }

        [Column("产品名称")]
        public string ProductName { get; set; }

        [Column("产品代码")]
        public string ProductCode { get; set; }

        [Column("规格")]
        public string Specification { get; set; }

        [Column("单位")]
        public string Unit { get; set; }

        [Column("下单量")]
        public string Count { get; set; }
        
        [Column("单据编号")]
        public string No { get; set; }

        [Column("预计到货时间")]
        public string ExpectedArrivalTime { get; set; }

        [Column("签收时间")]
        public string ReceivedTime { get; set; }

        [Column("司机信息")]
        public string DriverInfo { get; set; }

        [Column("发货仓")]
        public string DeliveryWarehouse { get; set; }

        [Column("运输方式")]
        public string TransportMethod { get; set; }

        [Column("承运单位")]
        public string Company { get; set; }

        [Column("释放时间")]
        public string ReleasedTime { get; set; }

        [Column("PGI 时间")]
        public string PgiTime { get; set; }

        [Column("发票号")]
        public string InvoiceNo { get; set; }

        [Column("GTS发票号")]
        public string GtsNo { get; set; }

        [Column("开票时间")]
        public string InvoiceTime { get; set; }

        [Column("合同上传时间")]
        public string ContractTime { get; set; }
    }
}
