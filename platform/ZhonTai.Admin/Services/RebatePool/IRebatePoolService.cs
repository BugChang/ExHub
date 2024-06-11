using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.RebatePool.Dto;

namespace ZhonTai.Admin.Services.RebatePool
{
    public interface IRebatePoolService
    {
        Task<PageOutput<RebateGetPageOutput>> GetPageAsync(PageInput<RebateGetPageInput> pageInput);

        Task<RebateGetPageOutput> GetAsync(string soldCode, string productCode);

        Task<List<RebateUploadOutput>> ImportAsync(IFormFile formFile);
    }
}
