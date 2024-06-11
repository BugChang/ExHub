using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Notice
{
    [Table(Name = "ad_notice_file")]
    public class NoticeFileEntity : EntityBase
    {
        public long NoticeId { get; set; }

        public long FileId { get; set; }

    }
}
