using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Order.Dto
{
    /// <summary>
    /// 订单主要项
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 箱规
        /// </summary>
        public int BoxSize { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        public string CancelReason { get; set; }
    }
}
