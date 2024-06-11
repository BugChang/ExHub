using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Protocol;

namespace ZhonTai.Admin.Repositories.Protocol
{
    public class ProtocolRepository : AdminRepositoryBase<ProtocolEntity>, IProtocolRepository
    {
        public ProtocolRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
