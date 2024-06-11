namespace ZhonTai.Admin.Services.Home.Dto
{
    public class HomeGetSummaryOutput
    {
        /// <summary>
        /// 经销商数量
        /// </summary>
        public int DealerCount { get; set; }

        /// <summary>
        /// 待处理订单需求
        /// </summary>
        public int OrderDemandCount { get; set; }
        /// <summary>
        ///  当月订单
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 当月订单采购量
        /// </summary>
        public int OrderProductCount { get; set; }
    }
}
