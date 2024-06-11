using System.Collections.Generic;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.ProductPrice;

namespace ZhonTai.Admin.Repositories.Dealer
{
    public class DealerRepository : AdminRepositoryBase<DealerEntity>, IDealerRepository
    {
        public DealerRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }

        public Task<DealerEntity> GetWithAddressAsync(string soldCode)
        {
            return Select.Where(a => a.SoldCode == soldCode)
                .IncludeMany(a => a.Addresses)
                 .ToOneAsync();
        }

        /// <summary>
        /// 检查经销商和产品是否有单价
        /// </summary>
        /// <param name="soldCode"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public async Task<bool> GetWithPriceAsync(string soldCode, string productCode)
        {
            var checkResult = false;
            var prices = await Orm.GetRepositoryBase<ProductPriceEntity>()
                .Where(p => p.SoldCode == soldCode && p.ProductCode == productCode)
                .ToListAsync();
            if (prices.Count > 0)
            {
                checkResult = true;
            }
            return checkResult;
        }

        public Task<List<string>> GetCurrentUserSoldCodesAsync()
        {
            return Select.Where(a => a.BizUserId == User.Id || a.RegionManagerUserId == User.Id)
                .Where(a => a.Status == DealerStatus.Active)
                .ToListAsync(a => a.SoldCode);
        }
    }
}
