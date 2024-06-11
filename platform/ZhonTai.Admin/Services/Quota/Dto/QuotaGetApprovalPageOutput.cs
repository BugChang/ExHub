using System;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Quota;
using ZhonTai.Common.Extensions;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    /// <summary>
    /// 配额审批列表
    /// </summary>
    public class QuotaGetApprovalPageOutput
    {
        /// <summary>
        /// 审批Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 经销商类别
        /// </summary>
        public DealerType DealerType { get; set; }

        /// <summary>
        /// 经销商类别描述
        /// </summary>
        public string DealerTypeDesc => DealerType.ToDescription();

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 当月总配额
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 配额总数调整
        /// </summary>
        public int TotalChange { get; set; }

        /// <summary>
        /// 调整后配额总数
        /// </summary>
        public int TotalAfterChange => TotalCount + TotalChange;

        /// <summary>
        /// 待审批配额
        /// </summary>
        public int PendingCount { get; set; }

        /// <summary>
        /// 已获批占比
        /// </summary>
        public string ApprovedPercentage => TotalCount != 0 ? ((decimal)ApprovedCount * 100 / TotalCount).ToString("F2") : "";


        /// <summary>
        /// 已获批配额
        /// </summary>
        public int ApprovedCount { get; set; }

        /// <summary>
        /// 预期释放配额
        /// </summary>
        public int ExceptReleaseCount => TotalAfterChange - PendingCount - ApprovedCount;

        /// <summary>
        /// 审批状态
        /// </summary>
        public ApproveStatus Status { get; set; }

        /// <summary>
        /// 审批状态描述
        /// </summary>
        public string StatusDesc => Status.ToDescription();

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        public string YearMonth { get; set; }

        /// <summary>
        /// 已获批下单
        /// </summary>
        public int PrCount { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 商务总监
        /// </summary>
        public string BizApproveUserName { get; set; }


        /// <summary>
        /// PE总监
        /// </summary>
        public string PeApproveUserName { get; set; }

        /// <summary>
        /// 允许撤回
        /// </summary>
        public bool AllowWithdraw { get; set; }
    }

   
}
