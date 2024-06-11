using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Quota.Dto;

namespace ZhonTai.Admin.Services.Quota
{
    public interface IQuotaService
    {
        public Task<int> GetCountAsync(string soldCode, string productCode);

        public Task<PageOutput<QuotaGetApprovalPageOutput>> GetApprovalPage(PageInput<QuotaGetApprovalPageInput> input);

    }
}
