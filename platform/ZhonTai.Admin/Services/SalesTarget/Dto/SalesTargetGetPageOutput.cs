using System;
using System.ComponentModel;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.SalesTarget;


namespace ZhonTai.Admin.Services.SalesTarget.Dto
{
    public class SalesTargetGetPageOutput : SalesTargetAddInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EffectiveStatus Status { get; set; }

        public string StatusDesc  => Status.ToDescriptionOrString();

        /// <summary>
        /// 有警告
        /// </summary>
        public bool HasWarning { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 总配额
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedUserName { get; set; }


        /// <summary>
        /// 更新人真实姓名
        /// </summary>
        public string ModifiedUserRealName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
    }
}
