using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.OrderDemand.Dto;

namespace ZhonTai.Admin.Services.OrderDemand
{
    public interface IOrderDemandService
    {
        Task<PageOutput<OrderDemandGetPageOutput>> GetPageAsync(PageInput<OrderDemandGetPageInput> input);

        Task<long> SubmitAsync(OrderDemandInsertOrUpdateInput request);

        Task<long> SaveAsync(OrderDemandInsertOrUpdateInput request);

        Task<OrderDemandGetCombinedOutput> GetCombinedAsync(long id);

        Task CompleteAsync(long id);

        Task ReturnAsync(long id);

        Task<string> DeleteAsync(long key);
    }
}
