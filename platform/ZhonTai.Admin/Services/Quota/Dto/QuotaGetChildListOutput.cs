using System;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetChildListOutput
    {
        /// <summary>
        /// 配额更新量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
    }
}
