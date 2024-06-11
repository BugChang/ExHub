using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Repositories.Order
{
    public class InvoiceRepository : AdminRepositoryBase<InvoiceEntity>, IInvoiceRepository
    {
        public InvoiceRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
