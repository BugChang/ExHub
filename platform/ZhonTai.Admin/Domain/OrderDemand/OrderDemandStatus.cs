using System.ComponentModel;

namespace ZhonTai.Admin.Domain.OrderDemand
{
    public enum OrderDemandStatus
    {
        [Description("未提交")]
        Saved = 0,
        [Description("已提交")]
        Submitted = 1,
        [Description("已修改")]
        Edited = 2,
        [Description("已删除")]
        Deleted = 3,
        [Description("已完成")]
        Completed = 4,
        [Description("已退回")]
        Returned = 5
    }
}
