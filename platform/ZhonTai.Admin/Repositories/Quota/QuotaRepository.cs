using MimeKit;
using NPOI.SS.Formula.Functions;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Domain.Quota;

namespace ZhonTai.Admin.Repositories.Quota
{
    public class QuotaRepository : AdminRepositoryBase<QuotaEntity>, IQuotaRepository
    {
        public QuotaRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
