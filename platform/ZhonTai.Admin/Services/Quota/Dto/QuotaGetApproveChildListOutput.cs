using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetApproveChildListOutput
    {

        public string ProductCode { get; set; }


        public string ProductName { get; set; }

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
        /// 预期释放配额
        /// </summary>
        public int ExceptReleaseCount => TotalAfterChange - ApprovedCount - PendingCount;

        /// <summary>
        /// 已获批占比
        /// </summary>
        public string ApprovedPercentage => TotalCount != 0 ? ((decimal)ApprovedCount * 100 / TotalCount).ToString("F2") : "";


        /// <summary>
        /// 已获批配额
        /// </summary>
        public int ApprovedCount { get; set; }

    }
}
