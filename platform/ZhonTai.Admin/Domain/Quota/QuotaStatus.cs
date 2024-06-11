using System.ComponentModel;

namespace ZhonTai.Admin.Domain.Quota
{
    public enum QuotaStatus
    {
        /// <summary>
        /// 审批中
        /// </summary>
        [Description("审批中")]
        InApprove = 0,
        /// <summary>
        /// 已通过
        /// </summary>
        [Description("已通过")]
        Approved = 1,
        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("已拒绝")]
        Decline = 2,
        /// <summary>
        /// 已撤回
        /// </summary>
        [Description("已撤回")]
        Withdraw = 3,
    }
}
