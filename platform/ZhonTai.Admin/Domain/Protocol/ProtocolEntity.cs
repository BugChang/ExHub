using System;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.File;

namespace ZhonTai.Admin.Domain.Protocol
{
    [Table(Name = "ad_protocol")]
    public class ProtocolEntity : EntityBase
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProtocolName { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }


        [Navigate(nameof(FileId))]
        public FileEntity File { get; set; }
    }
}
