using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Product.Dto
{
    public class ProductGetCombinedInfoOutput : ProductDto
    {
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 剩余配额
        /// </summary>
        public int QuotaCount { get; set; }

        /// <summary>
        /// 返利池金额
        /// </summary>
        public decimal RebatePoolAmount { get; set; }
    }
}
