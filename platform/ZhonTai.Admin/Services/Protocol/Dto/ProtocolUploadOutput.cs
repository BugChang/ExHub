using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Protocol.Dto
{
    public class ProtocolUploadOutput
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    public class ProtocolUploadDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// SoldToCode
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProtocolName { get; set; }
    }
}
