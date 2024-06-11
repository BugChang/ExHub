using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.OrderDemand;

namespace ZhonTai.Admin.Repositories.OrderDemand
{
    public class OrderDemandItemRepository : AdminRepositoryBase<OrderDemandItemEntity>, IOrderDemandItemRepository
    {
        public OrderDemandItemRepository(UnitOfWorkManagerCloud uown) : base(uown)
        {
        }
    }
}
