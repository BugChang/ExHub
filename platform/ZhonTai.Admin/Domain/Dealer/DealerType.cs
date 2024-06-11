using System.ComponentModel;

namespace ZhonTai.Admin.Domain.Dealer
{
    public enum DealerType
    {
        [Description("一般经销商")]
        Normal = 0,
        [Description("重点经销商")]
        Emphasis = 1,
    }
}
