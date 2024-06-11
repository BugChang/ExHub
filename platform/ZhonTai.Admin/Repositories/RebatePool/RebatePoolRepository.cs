using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.RebatePool;

namespace ZhonTai.Admin.Repositories.RebatePool
{
    public class RebatePoolRepository : AdminRepositoryBase<RebatePoolEntity>, IRebatePoolRepository
    {
        public RebatePoolRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
