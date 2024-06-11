using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Quota;

namespace ZhonTai.Admin.Repositories.Quota
{
    public class QuotaApprovalRepository : AdminRepositoryBase<QuotaApprovalEntity>, IQuotaApprovalRepository
    {
        public QuotaApprovalRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
