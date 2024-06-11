using System.ComponentModel;

namespace ZhonTai.Admin.Domain.Dealer
{
    public enum DealerStatus
    {
        [Description("Inactive")]
        Inactive = 0,
        [Description("Active")]
        Active = 1,
    }
}
