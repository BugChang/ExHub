using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Order.Dto;

public class OrderGetPageOutput : OrderDto
{

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 创建人真实姓名
    /// </summary>
    public string CreatedUserRealName { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public string ModifiedUserName { get; set; }


    /// <summary>
    /// 更新人真实姓名
    /// </summary>
    public string ModifiedUserRealName { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? ModifiedTime { get; set; }

    /// <summary>
    /// 合同文件Id
    /// </summary>
    public long? ContractFileId { get; set; }

    /// <summary>
    /// 合同ID
    /// </summary>
    public long? ContractId { get; set; }
}
