using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Dealer;

namespace ZhonTai.Admin.Repositories.Dealer
{
    public class DealerAddressMapRepository : AdminRepositoryBase<DealerAddressMapEntity>, IDealerAddressMapRepository
    {
        public DealerAddressMapRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
