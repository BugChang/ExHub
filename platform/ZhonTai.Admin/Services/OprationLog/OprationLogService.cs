using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZhonTai.Common.Helpers;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.OprationLog.Dto;
using ZhonTai.Admin.Domain.OprationLog;
using ZhonTai.Admin.Domain;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using ZhonTai.Admin.Core.Consts;
using System;
using System.IO;
using Npoi.Mapper;

namespace ZhonTai.Admin.Services.OprationLog;

/// <summary>
/// 操作日志服务
/// </summary>
[Order(200)]
[DynamicApi(Area = AdminConsts.AreaName)]
public class OprationLogService : BaseService, IOprationLogService, IDynamicApi
{
    private readonly IHttpContextAccessor _context;
    private readonly IOprationLogRepository _oprationLogRepository;

    public OprationLogService(
        IHttpContextAccessor context,
        IOprationLogRepository oprationLogRepository
    )
    {
        _context = context;
        _oprationLogRepository = oprationLogRepository;
    }

    /// <summary>
    /// 查询分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PageOutput<OprationLogListOutput>> GetPageAsync(PageInput<LogGetPageDto> input)
    {
        var userName = input.Filter.CreatedUserName;
        var beginDate = input.Filter.BeginDate;
        var endDate = input.Filter.EndDate?.AddDays(1) ?? input.Filter.EndDate;
        if (beginDate == null != (endDate == null))
        {
            throw ResultOutput.Exception("请选择完整的时间范围");
        }
        var list = await _oprationLogRepository.Select
        .WhereDynamicFilter(input.DynamicFilter)
        .WhereIf(userName.NotNull(), a => a.CreatedUserName.Contains(userName))
        .WhereIf(input.Filter.OperationName.NotNull(), a => a.ApiLabel.Contains(input.Filter.OperationName))
        .WhereIf(input.Filter.Status.HasValue, a => a.Status == input.Filter.Status)
        .WhereIf(beginDate.HasValue, a => a.CreatedTime >= beginDate && a.CreatedTime <= endDate)
        .Count(out var total)
        .OrderByDescending(true, c => c.Id)
        .Page(input.CurrentPage, input.PageSize)
        .ToListAsync<OprationLogListOutput>();

        var data = new PageOutput<OprationLogListOutput>()
        {
            List = list,
            Total = total
        };

        return data;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<long> AddAsync(OprationLogAddInput input)
    {
        if (_context.HttpContext != null)
        {
            string ua = _context.HttpContext.Request.Headers["User-Agent"];
            if (ua.NotNull())
            {
                var client = UAParser.Parser.GetDefault().Parse(ua);
                var device = client.Device.Family;
                device = device.ToLower() == "other" ? "" : device;
                input.Browser = client.UA.Family;
                input.Os = client.OS.Family;
                input.Device = device;
                input.BrowserInfo = ua;
            }
        }

        input.Name = User.Name;
        input.IP = IPHelper.GetIP(_context?.HttpContext?.Request);

        var entity = Mapper.Map<OprationLogEntity>(input);
        await _oprationLogRepository.InsertAsync(entity);

        return entity.Id;
    }


    /// <summary>
    /// 导出
    /// </summary>
    /// <returns></returns>
    public async Task<ActionResult> ExportAsync(LogGetPageDto input)
    {
        var userName = input.CreatedUserName;
        var beginDate = input.BeginDate;
        var endDate = input.EndDate?.AddDays(1) ?? input.EndDate;
        if (beginDate == null || endDate == null)
        {
            throw ResultOutput.Exception("请选择完整的时间范围");
        }

        //TimeSpan difference = endDate.Value - beginDate.Value;
        //if (difference.TotalDays >= 30)
        //{
        //    throw ResultOutput.Exception("最多导出30天的数据");
        //}
        var list = await _oprationLogRepository.Select
            .Where(a => a.CreatedTime >= beginDate && a.CreatedTime <= endDate)
            .WhereIf(userName.NotNull(), a => a.CreatedUserName.Contains(userName))
            .WhereIf(input.OperationName.NotNull(), a => a.ApiLabel.Contains(input.OperationName))
            .WhereIf(input.Status.HasValue, a => a.Status == input.Status)
            .OrderByDescending(true, c => c.Id)
            .ToListAsync<OprationLogListOutput>();

        var mapper = new Mapper();
        MemoryStream stream = new MemoryStream();

        mapper.Save(stream, list, "Sheet1", false);

        return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"操作日志导出_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        };
    }
}