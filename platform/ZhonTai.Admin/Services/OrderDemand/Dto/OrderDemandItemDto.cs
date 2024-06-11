using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandItemDto
    {
        /// <summary>
        /// 产品编码
        /// </summary>
        [Required(ErrorMessage = "The Product Code is required！")]
        public string ProductCode { get; set; }

        public string ProductName { get; set; } 

        /// <summary>
        /// 需求量
        /// </summary>
        public int NeedCount { get; set; }

        /// <summary>
        /// 返利金额 
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// 接受零头箱
        /// </summary>
        public bool AcceptPartialBox { get; set; }

        /// <summary>
        /// 是否使用折扣
        /// </summary>
        public bool IsUseDiscount { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 箱规
        /// </summary>
        public int BoxSize { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

    }

    public class OrderDemandItemWithOrderDto : OrderDemandItemDto
    {
        /// <summary>
        /// 订单
        /// </summary>
        public List<OrderDemandOrderDto> Orders { get; set; } = new();

       

        public decimal TotalSoCount => Orders.Sum(a => a.Count);
    }
}
