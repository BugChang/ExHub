using System;

namespace ZhonTai.Admin.Domain;

public class LogGetPageDto
{
    /// <summary>
    /// 创建者
    /// </summary>
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 操作名称
    /// </summary>
    public string OperationName { get; set; }

    /// <summary>
    /// 操作状态
    /// </summary>
    public bool? Status { get; set; }


    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginDate { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>

    public DateTime? EndDate { get; set; }
}