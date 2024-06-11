using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.SerialNo;

namespace ZhonTai.Admin.Repositories.SerialNo
{
    public class SerialNoRepository : AdminRepositoryBase<SerialNoEntity>, ISerialNoRepository
    {
        public SerialNoRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
