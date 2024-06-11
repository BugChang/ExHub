using System;
using System.Collections.Generic;
using System.Linq;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandGetOutput : OrderDemandDto
    {
        /// <summary>
        /// 产品项
        /// </summary>
        public List<OrderDemandItemWithOrderDto> Items { get; set; } = new();

        /// <summary>
        /// PR总数
        /// </summary>
        public int TotalPrCount => Items.Sum(a => a.NeedCount);

        /// <summary>
        /// SO总数
        /// </summary>
        public int TotalSoCount => Items.Sum(a => a.Orders.Select(b => b.Count).Sum());

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }
    }
}
