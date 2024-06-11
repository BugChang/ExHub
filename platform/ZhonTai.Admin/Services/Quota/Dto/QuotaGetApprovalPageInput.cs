using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Quota;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    /// <summary>
    /// 审批列表分页查询
    /// </summary>
    public class QuotaGetApprovalPageInput
    {
        /// <summary>
        /// 经销商代码/名称
        /// </summary>
        public string DealerKey { get; set; }

        /// <summary>
        /// 经销商类别
        /// </summary>
        public DealerType? DealerType { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public ApproveStatus? Status { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        public string YearMonth { get; set; }
    }

    public enum ApproveStatus
    {
        /// <summary>
        /// 审批中
        /// </summary>
        InApprove = 0,

        /// <summary>
        /// 已审批
        /// </summary>
        Approved = 1
    }
}
