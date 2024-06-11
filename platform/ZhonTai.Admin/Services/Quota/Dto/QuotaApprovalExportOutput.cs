using Npoi.Mapper.Attributes;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaApprovalExportOutput
    {
        [Column("经销商类别")]
        public string DealerType { get; set; }
        [Column("经销商代码")]
        public string SoldCode { get; set; }
        [Column("经销商名称")]
        public string DealerName { get; set; }
        [Column("产品代码")]
        public string ProductCode { get; set; }
        [Column("产品名称")]
        public string ProductName { get; set; }
        [Column("配额总调整数")]
        public string TotalChange { get; set; }
        [Column("待审批配额")]
        public string PendingCount { get; set; }
        [Column("操作")]
        public string Status { get; set; }
        [Column("生效日期")]
        public string EffectiveDate { get; set; }
        [Column("失效日期")]
        public string ExpirationDate { get; set; }
        [Column("所属年月")]
        public string YearMonth { get; set; }
        [Column("操作人")]
        public string ModifiedUserRealName { get; set; }

        [Column("操作时间")]
        public string ModifiedTime { get; set; }
    }
}
