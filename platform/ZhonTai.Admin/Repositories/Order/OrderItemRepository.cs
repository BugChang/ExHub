using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Repositories.Order
{
    public class OrderItemRepository : AdminRepositoryBase<OrderItemEntity>, IOrderItemRepository
    {
        public OrderItemRepository(UnitOfWorkManagerCloud uowm) : base(uowm) 
        {
        }
    }
}
