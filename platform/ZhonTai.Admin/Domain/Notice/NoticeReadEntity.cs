using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Notice
{
    [Table(Name = "ad_notice_read")]
    public class NoticeReadEntity : EntityBase
    {
        public long NoticeId { get; set; }

        public long UserId { get; set; }
    }
}
