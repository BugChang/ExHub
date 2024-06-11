using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Protocol.Dto
{
    public class ProtocolGetPageOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProtocolName { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 协议文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 协议文件ID
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedUserName { get; set; }

        /// <summary>
        /// 更新人真实姓名
        /// </summary>
        public string ModifiedUserRealName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
    }
}
