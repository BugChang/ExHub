using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Product;

namespace ZhonTai.Admin.Domain.ProductPrice
{
    /// <summary>
    /// 经销商产品价目
    /// </summary>
    [Table(Name = "ad_product_price")]
    public class ProductPriceEntity : EntityBase
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Amount { get; set; }

        [Navigate(nameof(SoldCode), TempPrimary = nameof(Dealer.SoldCode))]
        public DealerEntity Dealer { get; set; }


        [Navigate(nameof(ProductCode), TempPrimary = nameof(Product.Code))]
        public ProductEntity Product { get; set; }
    }
}
