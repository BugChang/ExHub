using FreeSql.DataAnnotations;
using System;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Domain.File;

namespace ZhonTai.Admin.Domain.Contract
{
    [Table(Name = "ad_contract")]
    public class ContractEntity : EntityBase
    {
        /// <summary>
        /// 订单代码
        /// </summary>
        public string SoCode { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// 文件实体
        /// </summary>
        [Navigate(nameof(FileId))]
        public FileEntity File { get; set; }
    }
}
