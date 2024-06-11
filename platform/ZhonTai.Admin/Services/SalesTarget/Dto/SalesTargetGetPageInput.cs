using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.SalesTarget;

namespace ZhonTai.Admin.Services.SalesTarget.Dto
{
    public class SalesTargetGetPageInput
    {
        /// <summary>
        /// 所属年月
        /// </summary>
        public string YearMonth { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EffectiveStatus? Status { get; set; }

    }
}
