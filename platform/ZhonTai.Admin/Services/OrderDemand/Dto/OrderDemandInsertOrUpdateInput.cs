using System.Collections.Generic;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    /// <summary>
    /// 新增订单需求
    /// </summary>
    public class OrderDemandInsertOrUpdateInput : OrderDemandDto
    {
        /// <summary>
        /// 订单项列表
        /// </summary>
        public List<OrderDemandItemDto> Items { get; set; }
    }
}
