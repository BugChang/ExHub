using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Protocol.Dto;

namespace ZhonTai.Admin.Services.Protocol
{
    public interface IProtocolService
    {
        Task<IResultOutput<List<ProtocolUploadOutput>>> CheckAsync(List<string> fileNames);

        Task<string> UploadAsync([FromForm] List<IFormFile> files);

        Task<PageOutput<ProtocolGetPageOutput>> GetPageAsync(PageInput<ProtocolGetPageInput> input);
    }
}
