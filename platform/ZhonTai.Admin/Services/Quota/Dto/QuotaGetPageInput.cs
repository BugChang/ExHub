using ZhonTai.Admin.Core.Enums;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetPageInput
    {
        /// <summary>
        /// 经销商代码/名称
        /// </summary>
        public string DealerKey { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 生效状态
        /// </summary>
        public EffectiveStatus? EffectiveStatus { get; set; }

        /// <summary>
        /// 所属年月
        /// </summary>
        public string YearMonth { get; set; }
    }
}
