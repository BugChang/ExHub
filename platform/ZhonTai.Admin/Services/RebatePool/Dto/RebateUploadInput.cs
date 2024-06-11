using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.RebatePool.Dto
{
    /// <summary>
    /// 返利池上传输入
    /// </summary>
    public class RebateUploadInput
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }


        /// <summary>
        /// 返利金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 最后上传时间
        /// </summary>
        public DateTime? LastUploadedTime { get; set; }
    }
}
