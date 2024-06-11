using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetApproveLogPageInput
    {
        /// <summary>
        /// 经销商代码/名称
        /// </summary>
        public string DealerKey { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        public string YearMonth { get; set; }

        /// <summary>
        /// 操作时间开始
        /// </summary>
        public DateTime? OperateTimeFrom { get; set; }


        /// <summary>
        /// 操作时间结束
        /// </summary>
        public DateTime? OperateTimeTo { get; set; }
    }
}
