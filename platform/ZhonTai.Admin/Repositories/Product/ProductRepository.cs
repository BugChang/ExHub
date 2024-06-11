using System.Collections.Generic;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Domain.ProductPrice;

namespace ZhonTai.Admin.Repositories.Product
{
    public class ProductRepository(UnitOfWorkManagerCloud uowm)
        : AdminRepositoryBase<ProductEntity>(uowm), IProductRepository
    {
        public Task ImportPriceAsync(IEnumerable<ProductPriceEntity> productPriceEntities)
        {
            var rep = Orm.GetRepositoryBase<ProductPriceEntity>();
            rep.DeleteAsync(a => true);
            return rep.InsertAsync(productPriceEntities);
        }
    }
}
