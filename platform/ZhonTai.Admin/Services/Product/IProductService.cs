using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Dealer.Dto;
using ZhonTai.Admin.Services.Product.Dto;

namespace ZhonTai.Admin.Services.Product
{
    public interface IProductService
    {
        public Task<List<ProductListOutput>> GetDealerListAsync(string soldCode, string key);

        public Task<List<ProductListOutput>> GetListAsync();
        public Task<ProductGetCombinedInfoOutput> GetCombinedInfoAsync(string code, string soldCode);

        public Task<string> ImportAsync(IFormFile formFile);

        Task<List<DealerImportOutput>> ImportPriceAsync(IFormFile formFile);
    }
}
