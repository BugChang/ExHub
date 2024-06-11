using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.OrderDemand;

namespace ZhonTai.Admin.Repositories.OrderDemand
{
    public class OrderDemandRepository(UnitOfWorkManagerCloud uown)
        : AdminRepositoryBase<OrderDemandEntity>(uown), IOrderDemandRepository
    {
        public async Task<string> DeleteItemAsync(long id)
        {
            var rep = Orm.GetRepositoryBase<OrderDemandItemEntity>();
            await rep.DeleteAsync(p => p.OrderDemandId == id);
            return "Success!";

        }

        /// <summary>
        /// 根据PRCode返回金额
        /// </summary>
        /// <param name="prCode"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public async Task<decimal> GetAmountAsync(string prCode, string productCode)
        {
            var orderDemandId = Select.Where(p => p.PrCode == prCode).ToOne().Id;
            var item = await Orm.GetRepositoryBase<OrderDemandItemEntity>().Select
                .Where(p => p.OrderDemandId == orderDemandId && p.ProductCode == productCode)
                .ToOneAsync();
            return item.Amount * item.NeedCount;
        }

        /// <summary>
        /// 根据PRCode返回返利金额
        /// </summary>
        /// <param name="prCode"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public async Task<decimal> GetDiscountAmountAsync(string prCode, string productCode)
        {
            var orderDemandId = Select.Where(p => p.PrCode == prCode).ToOne().Id;
            var item = await Orm.GetRepositoryBase<OrderDemandItemEntity>().Select
                .Where(p => p.OrderDemandId == orderDemandId && p.ProductCode == productCode)
                .ToOneAsync();
            return item.RebateAmount;
        }
    }
}

