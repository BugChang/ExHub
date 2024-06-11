using System;
using System.Collections.Generic;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Common.Extensions;


namespace ZhonTai.Admin.Services.Notice.Output
{
    public class NoticeDetailOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 共享范围
        /// </summary>
        public SharedScope Scope { get; set; }

        /// <summary>
        /// 共享范文字
        /// </summary>
        public string ScopeStr
        {
            get
            {
                return Scope.ToDescription();
            }
        }

        /// <summary>
        /// 发布部门
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 创建人用户名
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>

        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 最新变更时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        /// 阅读创建时间
        /// </summary>
        public DateTime? ReadCreatedTime { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public List<FileDetail> files { get; set; } = new List<FileDetail>();
    }

    public class FileDetail
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }
    }
}
