using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Notice;

namespace ZhonTai.Admin.Repositories.Notice
{
    public class NoticeReadRepository : AdminRepositoryBase<NoticeReadEntity>, INoticeReadRepository
    {
        public NoticeReadRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
