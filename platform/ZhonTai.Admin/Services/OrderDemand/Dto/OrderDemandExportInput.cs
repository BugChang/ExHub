using System;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandExportInput
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime BeginDate { get; set; } = DateTime.Now.AddMonths(-6).Date;
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}
