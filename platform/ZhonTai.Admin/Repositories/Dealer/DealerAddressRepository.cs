using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Dealer;

namespace ZhonTai.Admin.Repositories.Dealer
{
    public class DealerAddressRepository : AdminRepositoryBase<DealerAddressEntity>, IDealerAddressRepository
    {
        public DealerAddressRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
