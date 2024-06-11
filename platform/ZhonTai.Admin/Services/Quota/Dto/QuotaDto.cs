using System;
using ZhonTai.Admin.Core.Enums;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaDto
    {
        /// <summary>
        /// 经销商代码
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 配额数量
        /// </summary>
        public int Count { get; set; }

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
        public string YearMonth { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 生效状态
        /// </summary>
        public EffectiveStatus EffectiveStatus
        {
            get
            {
                if (DateTime.Today >= EffectiveDate && DateTime.Today < ExpirationDate)
                {
                    return EffectiveStatus.Active;
                }

                if (ExpirationDate <= DateTime.Today)
                {
                    return EffectiveStatus.InActive;
                }

                return EffectiveStatus.NotActive;
            }
        }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemainingCount { get; set; }


    }
}
