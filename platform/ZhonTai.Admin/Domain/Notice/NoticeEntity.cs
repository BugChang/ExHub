using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Core.Enums;

namespace ZhonTai.Admin.Domain.Notice
{
    [Table(Name = "ad_notice")]
    public class NoticeEntity : EntityBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column(StringLength = -1)]
        public string Content { get; set; }

        /// <summary>
        /// 共享范围
        /// </summary>
        public SharedScope Scope { get; set; }

        /// <summary>
        /// 发布部门
        /// </summary>
        public string OrgName { get; set; }

        [Navigate(nameof(NoticeFileEntity.NoticeId))]
        public List<NoticeFileEntity> Files { get; set; }
    }
}
