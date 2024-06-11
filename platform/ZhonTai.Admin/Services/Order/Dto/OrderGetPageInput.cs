using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Services.Order.Dto;

/// <summary>
/// 订单分页查询组合
/// </summary>
public class OrderGetPageInput
{
    /// <summary>
    /// 经销商名称/PRCode/SOCode
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus? Status { get; set; }

    /// <summary>
    /// Sap创建时间开始
    /// </summary>
    public DateTime? SapCreatedTimeFrom { get; set; }

    /// <summary>
    /// SAP创建时间结束
    /// </summary>
    public DateTime? SapCreatedTimeTo { get; set; }

    /// <summary>
    /// 区域
    /// </summary>
    public string Region { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedUserName { get; set; }
}
