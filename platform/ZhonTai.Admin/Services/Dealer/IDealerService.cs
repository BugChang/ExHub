using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZhonTai.Admin.Services.Dealer.Dto;

namespace ZhonTai.Admin.Services.Dealer
{
    public interface IDealerService
    {

        Task<List<DealerAddressListOutput>> GetAddressListAsync(string soldCode = "");

        Task<List<DealerGetListOutput>> GetListAsync(string key = "");

        Task<List<DealerImportOutput>> ImportAsync(IFormFile formFile);

        Task<string> ImportAddressAsync(IFormFile formFile);

        Task<List<string>> GetCurrentUserSoldCodesAsync();

    }
}
