﻿using SixLabors.ImageSharp.Drawing;
using System.ComponentModel;
using ZhonTai.Admin.Core.Attributes;

namespace ZhonTai.Admin.Core.Consts;

/// <summary>
/// 缓存键
/// </summary>
[ScanCacheKeys]
public static partial class CacheKeys
{
    /// <summary>
    /// 验证码 admin:captcha:guid
    /// </summary>
    [Description("验证码")]
    public const string Captcha = "admin:captcha:";

    /// <summary>
    /// 密码加密 admin:password:encrypt:guid
    /// </summary>
    [Description("密码加密")]
    public const string PassWordEncrypt = "admin:password:encrypt:";

    /// <summary>
    /// 用户权限 admin:user:permissions:用户主键
    /// </summary>
    [Description("用户权限")]
    public const string UserPermissions = "admin:user:permissions:";

    /// <summary>
    /// 数据权限 admin:user:data:permission:用户主键
    /// </summary>
    [Description("数据权限")]
    public const string DataPermission = "admin:user:data:permission:";

    /// <summary>
    /// 短信验证码 admin:sms:code:guid
    /// </summary>
    [Description("短信验证码")]
    public const string SmsCode = "admin:sms:code:";

    /// <summary>
    /// 经销商列表 admin:dealers
    /// </summary>
    [Description("经销商列表")]
    public const string Dealers = "admin:dealers";

    /// <summary>
    /// 经销商地址列表 admin:dealer:address
    /// </summary>
    [Description("经销商地址列表")]
    public const string DealerAddresses = "admin:dealer:address";

    /// <summary>
    /// 产品列表 admin:products
    /// </summary>
    [Description("产品列表")]
    public const string Products = "admin:products";

    /// <summary>
    /// 获取短信验证码缓存键
    /// </summary>
    /// <param name="mobile">手机号</param>
    /// <param name="code">唯一码</param>
    /// <returns></returns>
    public static string GetSmsCodeKey(string mobile, string code) => $"{SmsCode}{mobile}:{code}";

    /// <summary>
    /// 获取数据权限缓存键
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="apiPath">请求接口路径</param>
    /// <returns></returns>
    public static string GetDataPermissionKey(long userId, string apiPath = null)
    {
        if(apiPath.IsNull())
        {
            apiPath = AppInfo.CurrentDataPermissionApiPath;
        }

        return $"{DataPermission}{userId}{(apiPath.NotNull() ? (":" + apiPath) : "")}";
    }

    /// <summary>
    /// 获取数据权限模板
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <returns></returns>
    public static string GetDataPermissionPattern(long userId) => $"{DataPermission}{userId}*";
}