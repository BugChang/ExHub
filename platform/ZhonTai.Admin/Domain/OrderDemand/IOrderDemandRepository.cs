using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Repositories;
using ZhonTai.Admin.Domain.Dealer;

namespace ZhonTai.Admin.Domain.OrderDemand
{
    public interface IOrderDemandRepository : IRepositoryBase<OrderDemandEntity>
    {
        Task<string> DeleteItemAsync(long id);

        Task<decimal> GetAmountAsync(string prCode, string productCode);

        Task<decimal> GetDiscountAmountAsync(string prCode, string productCode);
    }
}
