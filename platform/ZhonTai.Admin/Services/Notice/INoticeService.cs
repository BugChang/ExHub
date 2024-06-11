using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Notice.Input;
using ZhonTai.Admin.Services.Notice.Output;

namespace ZhonTai.Admin.Services.Notice
{
    public interface INoticeService
    {
        Task<PageOutput<NoticeListOutput>> GetPageAsync(PageInput<NoticeGetListInput> input);

        Task<NoticeDetailOutput> GetAsync(long Id);

        Task DeleteAsync(long id);

        Task<long> AddAsync(NoticeAddInput input);

        Task<long> UpdateAsync(NoticeUpdInput input);
    }
}
