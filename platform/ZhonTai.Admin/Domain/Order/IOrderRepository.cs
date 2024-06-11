using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Repositories;
using ZhonTai.Admin.Domain.Document;

namespace ZhonTai.Admin.Domain.Order
{
    public interface IOrderRepository : IRepositoryBase<OrderEntity>
    {
    }
}
