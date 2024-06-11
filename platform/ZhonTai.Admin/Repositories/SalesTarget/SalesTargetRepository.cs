using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.SalesTarget;

namespace ZhonTai.Admin.Repositories.SalesTarget
{
    public class SalesTargetRepository:AdminRepositoryBase<SalesTargetEntity>,ISalesTargetRepository
    {
        public SalesTargetRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
