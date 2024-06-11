using System;
using System.ComponentModel.DataAnnotations;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetChildListInput
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        [Required(ErrorMessage = "SoldCode必填")]
        public string SoldCode { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        [Required(ErrorMessage = "ProductCode必填")]
        public string ProductCode { get; set; }

        /// <summary>
        ///生效日期
        /// </summary>
        [Required(ErrorMessage = "EffectiveDate必填")]
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [Required(ErrorMessage = "ExpirationDate必填")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Required(ErrorMessage = "CreatedUserName必填")]
        public string CreatedUserName { get; set; }
    }
}
