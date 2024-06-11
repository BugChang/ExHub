using System.ComponentModel;

namespace ZhonTai.Admin.Domain.SalesTarget
{
    public enum SalesTargetStatus
    {
        [Description("正常")]
        Active = 0,
        [Description("废弃")]
        InActive = 1
    }
}
