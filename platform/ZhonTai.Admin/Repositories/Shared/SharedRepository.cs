using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Notice;
using ZhonTai.Admin.Domain.Shared;

namespace ZhonTai.Admin.Repositories.Shared
{
    public class SharedRepository : AdminRepositoryBase<SharedEntity>, ISharedRepository
    {
        public SharedRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
