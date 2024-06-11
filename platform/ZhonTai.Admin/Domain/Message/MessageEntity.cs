using System;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Message
{
    [Table(Name = "ad_message")]
    public class MessageEntity : Entity, IDelete
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
        /// 收件人
        /// </summary>
        public long ReceiveUserId { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }
    }
}
