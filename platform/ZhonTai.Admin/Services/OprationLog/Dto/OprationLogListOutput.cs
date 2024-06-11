using System;
using Npoi.Mapper.Attributes;

namespace ZhonTai.Admin.Services.OprationLog.Dto;

public class OprationLogListOutput
{
    /// <summary>
    /// 编号
    /// </summary>
    [Column("编号")]
    public long Id { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Column("昵称")]
    public string NickName { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    [Column("创建者")]
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    [Column("接口名称")]
    public string ApiLabel { get; set; }

    /// <summary>
    /// 接口地址
    /// </summary>
    [Column("接口地址")]
    public string ApiPath { get; set; }

    /// <summary>
    /// 接口提交方法
    /// </summary>
    [Column("接口提交方法")]
    public string ApiMethod { get; set; }

    /// <summary>
    /// IP
    /// </summary>
    [Column("IP")]
    public string IP { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [Column("浏览器")]
    public string Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [Column("操作系统")]
    public string Os { get; set; }

    /// <summary>
    /// 设备
    /// </summary>
    [Column("Device")]
    public string Device { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    [Column("耗时（毫秒）")]
    public long ElapsedMilliseconds { get; set; }

    /// <summary>
    /// 操作状态
    /// </summary>
    [Column("操作状态")]
    public bool Status { get; set; }

    /// <summary>
    /// 操作消息
    /// </summary>
    [Column("操作消息")]
    public string Msg { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("创建时间")]
    public DateTime? CreatedTime { get; set; }

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