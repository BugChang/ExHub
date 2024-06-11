using System.ComponentModel;

namespace ZhonTai.Admin.Domain.Order
{
    public enum OrderStatus
    {
        [Description("已创建")]
        Created = 0,
        [Description("已释放")]
        Released = 1,
        [Description("已发运")]
        Shipped = 2,
        [Description("已签收")]
        Signed = 3,
        [Description("已删除")]
        Deleted = 4,
    }
}
