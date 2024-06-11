using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    /// <summary>
    /// 提交校验输出
    /// </summary>
    public class OrderDemandSubmitCheckOutput
    {
        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
