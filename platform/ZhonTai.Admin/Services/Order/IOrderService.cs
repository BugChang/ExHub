using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.Order.Dto;

namespace ZhonTai.Admin.Services.Order
{
    public interface IOrderService
    {
        Task<PageOutput<OrderGetPageOutput>> GetPageAsync(PageInput<OrderGetPageInput> input);

        Task<List<OrderGetOutput>> GetListAsync(string prCode);

        Task<OrderGetOutput> GetAsync(long id);

        Task<List<ImportOutput>> ImportAsync(IFormFile formFile);
    }
}
