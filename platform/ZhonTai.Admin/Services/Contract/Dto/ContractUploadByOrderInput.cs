using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Contract.Dto
{
    public class ContractUploadByOrderInput
    {
        /// <summary>
        /// 合同文件
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 订单SO号
        /// </summary>
        public string SoCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }
    }
}
