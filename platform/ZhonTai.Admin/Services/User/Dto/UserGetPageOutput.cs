using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ZhonTai.Admin.Domain.Role;
using ZhonTai.Admin.Domain.User;

namespace ZhonTai.Admin.Services.User.Dto;

public class UserGetPageOutput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 用户类型
    /// </summary>
    public UserType Type { get; set; }

    [JsonIgnore]
    public ICollection<RoleEntity> Roles { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public string[] RoleNames { get; set; }

    /// <summary>
    /// 是否主管
    /// </summary>
    public bool IsManager { get; set; }

    /// <summary>
    /// 启用
    /// </summary>
    public bool Enabled { get; set; }

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
    /// 经销商代码
    /// </summary>
    public string SoldCode { get; set; }
}