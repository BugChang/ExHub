using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Protocol;
using ZhonTai.Admin.Services.Dealer;
using ZhonTai.Admin.Services.File;
using ZhonTai.Admin.Services.Protocol.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.Protocol
{
    /// <summary>
    /// 协议服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class ProtocolService(Lazy<IProtocolRepository> protocolRepository,
            Lazy<IDealerService> dealerService,
            Lazy<IFileService> fileService)
        : BaseService, IProtocolService, IDynamicApi
    {
        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        [NonFormatResult]
        [HttpPost]
        public async Task<IResultOutput<List<ProtocolUploadOutput>>> CheckAsync(List<string> fileNames)
        {
            List<ProtocolUploadOutput> res = new List<ProtocolUploadOutput>();
            var protocolUploads = new List<ProtocolUploadDto>();
            foreach (var fileName in fileNames)
            {

                var extension = Path.GetExtension(fileName);
                var splitArr = fileName.Replace(extension, "").Split("_");
                if (extension != ".pdf" || splitArr.Length != 5)
                {
                    res.Add(new ProtocolUploadOutput
                    {
                        FileName = fileName,
                        ErrorMessage = "文件名格式有误"
                    });
                    continue;
                }

                var contractUploadOutput = new ProtocolUploadDto
                {
                    SoldCode = splitArr[0],
                    Name = splitArr[1],
                    FileName = fileName,
                    ProtocolName = splitArr[2],
                    StartTime = splitArr[3],
                    EndTime = splitArr[4],
                };
                if (
                    !DateTime.TryParseExact(contractUploadOutput.StartTime, "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out var startTime) ||
                    !DateTime.TryParseExact(contractUploadOutput.EndTime, "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out var endTime))
                {
                    res.Add(new ProtocolUploadOutput
                    {
                        FileName = fileName,
                        ErrorMessage = "文件名格式有误"
                    });
                    continue;
                }
                if (startTime >= endTime)
                {
                    res.Add(new ProtocolUploadOutput
                    {
                        FileName = fileName,
                        ErrorMessage = "失效日期应大于生效日期"
                    });
                    continue;
                }
                protocolUploads.Add(contractUploadOutput);
            }
            var soldCodes = protocolUploads.Select(r => r.SoldCode).ToList();
            var dealers = (await dealerService.Value.GetListAsync())
                .Where(a => soldCodes.Contains(a.SoldCode)).ToList();
            foreach (var item in protocolUploads)
            {
                var dealerCode = dealers.FirstOrDefault(t => t.SoldCode == item.SoldCode);
                if (dealerCode == null)
                {
                    res.Add(new ProtocolUploadOutput
                    {
                        FileName = item.FileName,
                        ErrorMessage = "经销商代码不存在"
                    });
                    continue;
                }
                dealerCode = dealers.FirstOrDefault(T => T.SoldCode == item.SoldCode && T.Name == item.Name);
                if (dealerCode == null)
                {
                    res.Add(new ProtocolUploadOutput
                    {
                        FileName = item.FileName,
                        ErrorMessage = "经销商代码与经销商名称不匹配"
                    });
                }
            }
            return res.Any() ? ResultOutput.NotOk($"识别到{fileNames.Count}个文件，其中{fileNames.Count - res.Count}个可提交绑定，{res.Count}个需检查文件名", res) : ResultOutput.Ok(res);
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<string> UploadAsync([FromForm] List<IFormFile> files)
        {
            //对文件名校验
            var checks = await CheckAsync(files.Select(T => T.FileName).ToList());
            if (!checks.Success)
            {
                throw ResultOutput.Exception("文件有误");
            }
            var protocols = new List<ProtocolEntity>();
            foreach (var item in files)
            {

                var extension = Path.GetExtension(item.FileName);
                var splitArr = item.FileName.Replace(extension, "").Split("_");
                var contractUploadOutput = new ProtocolUploadDto
                {
                    SoldCode = splitArr[0],
                    Name = splitArr[1],
                    FileName = item.FileName,
                    ProtocolName = splitArr[2],
                    StartTime = splitArr[3],
                    EndTime = splitArr[4],
                };

                DateTime.TryParseExact(contractUploadOutput.StartTime, "yyyyMMdd", null,
                    System.Globalization.DateTimeStyles.None, out var startTime);
                DateTime.TryParseExact(contractUploadOutput.EndTime, "yyyyMMdd", null,
                    System.Globalization.DateTimeStyles.None, out var endTime);


                var file = await fileService.Value.UploadFileAsync(item, fileDirectory: "Protocol");
                protocols.Add(new ProtocolEntity()
                {
                    SoldCode = contractUploadOutput.SoldCode,
                    DealerName = contractUploadOutput.Name,
                    FileId = file.Id,
                    File = file,
                    ProtocolName = contractUploadOutput.ProtocolName,
                    StartTime = startTime,
                    EndTime = endTime,
                });
            }
            protocolRepository.Value.DbContextOptions.EnableCascadeSave = true;
            foreach (var item in protocols)
            {
                await protocolRepository.Value.DeleteCascadeByDatabaseAsync(a =>
                    item.DealerName == a.DealerName &&
                    item.SoldCode == a.SoldCode &&
                    a.StartTime == item.StartTime &&
                    a.EndTime == item.EndTime);
            }

            if (protocols.Count > 0)
            {
                await protocolRepository.Value.InsertAsync(protocols);
            }
            return $"导入成功";
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input">查询组合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<ProtocolGetPageOutput>> GetPageAsync(PageInput<ProtocolGetPageInput> input)
        {
            var list = await protocolRepository.Value.Select
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Key),
                    p => p.ProtocolName.Contains(input.Filter.Key) || p.SoldCode.Contains(input.Filter.Key))
               .WhereIf(input.Filter.StartTime.HasValue && input.Filter.EndTime.HasValue,
                    p => p.StartTime <= input.Filter.EndTime && p.EndTime >= input.Filter.StartTime)
                .Count(out var total)
                .OrderByDescending(true, a => a.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync(a => new ProtocolGetPageOutput()
                {
                    Id = a.Id,
                    DealerName = a.DealerName,
                    SoldCode = a.SoldCode,
                    FileId = a.File.Id,
                    FileName = a.File.FileName,
                    ProtocolName = a.ProtocolName,
                    StartTime = a.StartTime.Value.ToString("yyyy/MM/dd"),
                    EndTime = a.EndTime.Value.ToString("yyyy/MM/dd"),
                    CreatedTime = a.CreatedTime,
                    CreatedUserName = a.CreatedUserName,
                    CreatedUserRealName = a.CreatedUserRealName,
                    ModifiedTime = a.ModifiedTime,
                    ModifiedUserName = a.ModifiedUserName,
                    ModifiedUserRealName = a.ModifiedUserRealName,
                });

            var data = new PageOutput<ProtocolGetPageOutput>()
            {
                List = list,
                Total = total
            };

            return data;
        }
    }
}
