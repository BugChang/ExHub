using System;

namespace ZhonTai.Admin.Services.Role.Dto;

public class RoleGetPageOutput
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool Hidden { get; set; }

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
}