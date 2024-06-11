using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Quota;
using ZhonTai.Common.Extensions;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetApproveLogPageOutput
    {
        /// <summary>
        /// 审批Id
        /// </summary>
        public long Id { get; set; }


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
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// 配额总数调整
        /// </summary>
        public int TotalChange { get; set; }


        /// <summary>
        /// 待审批配额
        /// </summary>
        public int PendingCount { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public QuotaStatus Status { get; set; }

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
        /// 操作人账号
        /// </summary>
        public string ModifiedUserName { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string ModifiedUserRealName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
    }
}
