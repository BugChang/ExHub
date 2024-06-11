using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaUploadInput
    {
        public string Region { get; set; }

        public string SoldCode { get; set; }

        public string DealerName { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public int TotalChange { get; set; }
        public int PendingCount { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        public string YearMonth { get; set; }
    }
}
