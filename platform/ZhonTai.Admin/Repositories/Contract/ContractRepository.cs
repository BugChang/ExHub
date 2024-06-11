using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Contract;
using ZhonTai.Admin.Domain.Document;

namespace ZhonTai.Admin.Repositories.Contract
{
    public class ContractRepository : AdminRepositoryBase<ContractEntity>, IContractRepository
    {
        public ContractRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
