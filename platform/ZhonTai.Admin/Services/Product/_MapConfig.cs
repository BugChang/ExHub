using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using ZhonTai.Admin.Domain.ProductPrice;
using ZhonTai.Admin.Services.Product.Dto;
using ZhonTai.Admin.Services.Role.Dto;

namespace ZhonTai.Admin.Services.Product
{
    /// <summary>
    /// 映射配置
    /// </summary>
    public class MapConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<ProductPriceEntity, ProductPriceDto>()
                .Map(dest => dest.DealerName, src => src.Dealer.Name)
                .Map(dest => dest.ProductName, src => src.Product.Name);
        }
    }
}
