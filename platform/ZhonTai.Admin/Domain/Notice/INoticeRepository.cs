using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Repositories;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Domain.Notice
{
    public interface INoticeRepository : IRepositoryBase<NoticeEntity>
    {
    }
}
