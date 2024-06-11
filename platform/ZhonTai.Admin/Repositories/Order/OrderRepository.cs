using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Repositories.Order
{
    public class OrderRepository : AdminRepositoryBase<OrderEntity>, IOrderRepository
    {
        public OrderRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
