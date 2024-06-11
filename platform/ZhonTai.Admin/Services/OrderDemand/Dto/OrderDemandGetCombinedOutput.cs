using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandGetCombinedOutput : OrderDemandDto
    {
        public List<OrderDemandCombinedItemDto> Items { get; set; }
    }
}
