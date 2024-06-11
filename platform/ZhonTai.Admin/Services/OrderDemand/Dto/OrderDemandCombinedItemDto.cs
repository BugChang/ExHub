using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandCombinedItemDto : OrderDemandItemDto
    {
        /// <summary>
        /// 返利池金额
        /// </summary>
        public decimal RebatePoolAmount { get; set; }

        /// <summary>
        /// 配额
        /// </summary>
        public int QuotaCount { get; set; }
    }
}
