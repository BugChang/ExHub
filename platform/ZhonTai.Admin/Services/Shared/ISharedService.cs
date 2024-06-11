using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Shared.Input;
using ZhonTai.Admin.Services.Shared.Output;

namespace ZhonTai.Admin.Services.Shared
{
    public interface ISharedService
    {
        Task<PageOutput<SharedListOutput>> GetPageAsync(PageInput<SharedListInput> input);

        Task DeleteAsync(long id);

        Task<long> UploadFileAsync(SharedUploadFileInput input);
    }
}
