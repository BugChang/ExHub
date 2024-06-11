using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Express;

namespace ZhonTai.Admin.Repositories.Express
{
    public class ExpressBatchRepository : AdminRepositoryBase<ExpressBatchEntity>, IExpressBatchRepository
    {
        public ExpressBatchRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
