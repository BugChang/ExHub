using System;
using Npoi.Mapper.Attributes;


namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandExportDto
    {
        [Column("PR Code")]
        public string PrCode { get; set; }
        [Column("经销商名称")]
        public string DealerName { get; set; }
        [Column("状态")]
        public string Status { get; set; }
        [Column("PR总数量")]
        public int PrCount { get; set; }
        [Column("SO总数量")]
        public int SoCount { get; set; }
        [Column("更新时间")]
        public DateTime? ModifiedTime { get; set; }
        [Column("创建人")]
        public string CreatedUserRealName { get; set; }
        [Column("区域")]
        public string Region { get; set; }
        [Column("Sold-to")]
        public string SoldCode { get; set; }
        [Column("Ship-to")]
        public string ShipCode { get; set; }
        [Column("Ship-to地址")]
        public string DealerAddress { get; set; }
        [Column("产品代码")]
        public string ProductCode { get; set; }
        [Column("产品名称")]
        public string ProductName { get; set; }
        [Column("规格")]
        public string Specification { get; set; }
        [Column("单价")]
        public decimal Amount { get; set; }
        [Column("数量")]
        public int ProductCount { get; set; }
        [Column("箱数")]
        public decimal BoxCount { get; set; }
        [Column("是否接受零头箱")]
        public string AcceptPartialBox { get; set; }
        [Column("折扣前金额")]
        public decimal TotalAmount { get; set; }
        [Column("是否使用票折")]
        public string IsUseDiscount { get; set; }
        [Column("折扣金额")]
        public decimal RebateAmount { get; set; }
        [Column("折扣后金额")]
        public decimal AfterDiscountAmount { get; set; }
        [Column("是否加急")]
        public string IsUrgent { get; set; }
        [Column("备注")]
        public string Remark { get; set; }
    }
}
