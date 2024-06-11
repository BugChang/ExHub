using System;

namespace ZhonTai.Admin.Services.RebatePool.Dto
{
    public class RebateGetPageOutput
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 返利池金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 返利更新时间
        /// </summary>
        public DateTime? RebateUpdateTime { get; set; }
    }
}
