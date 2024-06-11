using System.Collections.Generic;
using ZhonTai.Admin.Core.Repositories;
using ZhonTai.Admin.Domain.ProductPrice;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Domain.Product
{
    public interface IProductRepository : IRepositoryBase<ProductEntity>
    {
        System.Threading.Tasks.Task ImportPriceAsync(IEnumerable<ProductPriceEntity> productPriceEntities);
    }
}
