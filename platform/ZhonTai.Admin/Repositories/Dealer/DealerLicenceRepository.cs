using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Dealer;

namespace ZhonTai.Admin.Repositories.Dealer
{
    public class DealerLicenseRepository : AdminRepositoryBase<DealerLicenseEntity>, IDealerLicenseRepository
    {
        public DealerLicenseRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
