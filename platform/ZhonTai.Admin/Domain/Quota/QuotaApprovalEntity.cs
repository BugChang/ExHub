using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.File;

namespace ZhonTai.Admin.Domain.Quota
{
    [Table(Name = "ad_quota_approve")]
    public class QuotaApprovalEntity : EntityBase
    {
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 是否最新
        /// </summary>
        public bool IsLatest { get; set; }

        /// <summary>
        /// 总配额
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总配额调整
        /// </summary>
        public int TotalChange { get; set; }

        /// <summary>
        /// 获批配额
        /// </summary>
        public int ApprovedCount { get; set; }

        /// <summary>
        /// 待审批配额
        /// </summary>
        public int PendingCount { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 附件Id
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// 审批附件Id
        /// </summary>
        public long? ApproveFileId { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public QuotaStatus Status { get; set; }

        /// <summary>
        /// 商务总监审批人
        /// </summary>
        public string BizApproveUserName { get; set; }

        /// <summary>
        /// PE总监审批人
        /// </summary>
        public string PeApproveUserName { get; set; }

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
        /// 导航属性一对一关联文件Id
        /// </summary>
        [Navigate(nameof(FileId), TempPrimary = nameof(FileEntity.Id))]
        public FileEntity File { get; set; }

        /// <summary>
        /// 导航属性一对一关联文件Id
        /// </summary>
        [Navigate(nameof(ApproveFileId), TempPrimary = nameof(FileEntity.Id))]
        public FileEntity ApproveFile { get; set; }


        [Navigate(nameof(QuotaEntity.ApprovedId))]
        public List<QuotaEntity> Quotas { get; set; }


        [Navigate(nameof(SoldCode), TempPrimary = nameof(DealerEntity.SoldCode))]
        public DealerEntity Dealer { get; set; }
    }
}
