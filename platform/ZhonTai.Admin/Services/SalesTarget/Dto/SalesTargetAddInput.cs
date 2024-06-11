using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZhonTai.Admin.Core.Validators;

namespace ZhonTai.Admin.Services.SalesTarget.Dto
{
    public class SalesTargetAddInput
    {
        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        [Required]
        [ValidateRequired("所属年月不能为空")]
        public string YearMonth { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        public List<SalesTargetItemDto> Items { get; set; }
    }
}
