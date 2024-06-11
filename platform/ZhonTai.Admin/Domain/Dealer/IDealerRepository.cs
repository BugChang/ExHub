using System.Collections.Generic;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Repositories;

namespace ZhonTai.Admin.Domain.Dealer
{
    public interface IDealerRepository : IRepositoryBase<DealerEntity>
    {
        Task<DealerEntity> GetWithAddressAsync(string soldCode);
        Task<bool> GetWithPriceAsync(string soldCode, string productCode);

        Task<List<string>> GetCurrentUserSoldCodesAsync();
    }
}
