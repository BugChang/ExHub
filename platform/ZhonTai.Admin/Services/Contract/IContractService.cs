using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Contract.Dto;

namespace ZhonTai.Admin.Services.Contract
{
    public interface IContractService
    {
        Task<IResultOutput<List<ContractUploadOutput>>> CheckAsync(List<string> filenames);

        Task<string> UploadAsync([FromForm] List<IFormFile> files);

        Task<PageOutput<ContractGetListOutput>> GetPageAsync(PageInput<ContractGetListInPut> input);
    }
}
