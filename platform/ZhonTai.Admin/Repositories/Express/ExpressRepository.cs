using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Express;

namespace ZhonTai.Admin.Repositories.Express
{
    public class ExpressRepository :AdminRepositoryBase<ExpressEntity>,  IExpressRepository
    {
        public ExpressRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
