using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.ProductPrice;

namespace ZhonTai.Admin.Repositories.ProductPrice
{
    public class ProductPriceRepository : AdminRepositoryBase<ProductPriceEntity>, IProductPriceRepository
    {
        public ProductPriceRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
