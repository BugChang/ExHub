using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Notice;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Repositories.Notice
{
    public class NoticeRepository : AdminRepositoryBase<NoticeEntity>, INoticeRepository
    {
        public NoticeRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
