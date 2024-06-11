using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZhonTai.Admin.Core.Validators;

namespace ZhonTai.Admin.Services.SalesTarget.Dto
{
    public class SalesTargetUpdateInput
    {
        [Required]
        [ValidateRequired("请指定销售指标")]
        public long Id { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        [Required]
        [ValidateRequired("所属年月不能为空")]
        public string Month { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string AttachmentUrl { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        public List<SalesTargetItemDto> Items { get; set; }
    }
}
