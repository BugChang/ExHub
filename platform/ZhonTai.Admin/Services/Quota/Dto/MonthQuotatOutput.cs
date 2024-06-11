using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class MonthQuotaOutput
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 销售指标总数
        /// </summary>
        public int SaleTotalCount { get; set; }

        /// <summary>
        /// 当月配额总数
        /// </summary>
        public int QuotaTotalCount { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        public int Difference { get; set; }

    }

    public class MonthQuotaDataDto
    {
        /// <summary>
        /// 销售指标总数
        /// </summary>
        public int SaleTotalCount { get; set; }

        /// <summary>
        /// 当月配额总数
        /// </summary>
        public int QuotaTotalCount { get; set; }
    }
}
