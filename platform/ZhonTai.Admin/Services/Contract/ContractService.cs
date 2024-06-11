using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Contract;
using ZhonTai.Admin.Domain.Order;
using ZhonTai.Admin.Services.Contract.Dto;
using ZhonTai.Admin.Services.Dealer;
using ZhonTai.Admin.Services.File;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;
using Match = System.Text.RegularExpressions.Match;

namespace ZhonTai.Admin.Services.Contract
{
    /// <summary>
    /// 合同服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class ContractService(Lazy<IContractRepository> contractRepository,
            Lazy<IDealerService> dealerService,
            Lazy<IOrderRepository> orderRep,
            Lazy<IFileService> fileService)
        : BaseService, IContractService, IDynamicApi
    {
        private static readonly string ContractRegex = @"^(.*?)_(.*?)_(.*?)\.(pdf|png|jpg)$";
        private readonly IList<string> _contractAllowExtension = new List<string>() { ".pdf", ".png", ".jpg" };

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        [NonFormatResult]
        [HttpPost]
        public async Task<IResultOutput<List<ContractUploadOutput>>> CheckAsync(List<string> fileNames)
        {
            List<ContractUploadOutput> res = new List<ContractUploadOutput>();
            var contracts = new List<ContractUploadDto>();
            foreach (var item in fileNames)
            {
                Match match = Regex.Match(item, ContractRegex);
                if (match.Success)
                {
                    contracts.Add(new ContractUploadDto
                    {
                        SoldCode = match.Groups[1].Value,
                        SoCode = match.Groups[2].Value,
                        DealerName = match.Groups[3].Value,
                        FileName = item
                    });
                }
                else
                {
                    res.Add(new ContractUploadOutput
                    {
                        FileName = item,
                        ErrorMessage = "文件名格式有误"
                    });
                }
            }

            var soldCodes = contracts.Select(r => r.SoldCode).ToList();
            var dealers = (await dealerService.Value.GetListAsync())
                .Where(a => soldCodes.Contains(a.SoldCode)).ToList();

            var soCodes = contracts.Select(r => r.SoCode).ToList();
            var orders = await orderRep.Value.Select
                 .Where(a => soCodes.Contains(a.SoCode))
                 .ToListAsync();
            foreach (var item in contracts)
            {
                var dealerCode = dealers.FirstOrDefault(T => T.SoldCode == item.SoldCode);
                if (dealerCode == null)
                {
                    res.Add(new ContractUploadOutput
                    {
                        FileName = item.FileName,
                        ErrorMessage = "经销商代码不存在"
                    });
                    continue;
                }
                dealerCode = dealers.FirstOrDefault(a => a.SoldCode == item.SoldCode && a.Name == item.DealerName);
                if (dealerCode == null)
                {
                    res.Add(new ContractUploadOutput
                    {
                        FileName = item.FileName,
                        ErrorMessage = "经销商代码与经销商名称不匹配"
                    });
                    continue;
                }
                var order = orders.FirstOrDefault(T => T.SoCode == item.SoCode);
                if (order == null)
                {
                    res.Add(new ContractUploadOutput
                    {
                        FileName = item.FileName,
                        ErrorMessage = "订单SO号不存在"
                    });
                }
            }
            return res.Any() ? ResultOutput.NotOk($"识别到{fileNames.Count}个文件，其中{fileNames.Count - res.Count}个可提交绑定，{res.Count}个需检查文件名", res) : ResultOutput.Ok(res);
        }

        /// <summary>
        /// 批量上传
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
            List<ContractEntity> addContracts = new List<ContractEntity>();
            foreach (var item in files)
            {
                var file = await fileService.Value.UploadFileAsync(item, fileDirectory: "Contract");
                Match match = Regex.Match(item.FileName, ContractRegex);
                ContractUploadDto contractUploadOutput1 = new ContractUploadDto
                {
                    SoldCode = match.Groups[1].Value,
                    SoCode = match.Groups[2].Value,
                    DealerName = match.Groups[3].Value,
                    FileName = item.FileName
                };
                addContracts.Add(new ContractEntity()
                {
                    SoCode = contractUploadOutput1.SoCode,
                    SoldCode = contractUploadOutput1.SoldCode,
                    DealerName = contractUploadOutput1.DealerName,
                    FileId = file.Id
                });
            }

            contractRepository.Value.DbContextOptions.EnableCascadeSave = true;
            foreach (var item in addContracts)
            {
                await contractRepository.Value.DeleteCascadeByDatabaseAsync(a => item.SoCode == a.SoCode && item.SoldCode == a.SoldCode);
            }

            if (addContracts.Count > 0)
            {
                await contractRepository.Value.InsertAsync(addContracts);
            }
            return $"导入成功";
        }


        /// <summary>
        /// 按订单上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<string> UploadByOrderAsync([FromForm] ContractUploadByOrderInput input)
        {
            //对文件名校验
            var extension = Path.GetExtension(input.File.FileName);
            if (!_contractAllowExtension.Contains(extension))
                throw ResultOutput.Exception("不支持的文件格式");

            var file = await fileService.Value.UploadFileAsync(input.File, fileDirectory: "Contract");
            ContractEntity contract = new ContractEntity
            {
                SoldCode = input.SoldCode,
                SoCode = input.SoCode,
                DealerName = input.DealerName,
                FileId = file.Id
            };
            contractRepository.Value.DbContextOptions.EnableCascadeSave = true;
            await contractRepository.Value.DeleteCascadeByDatabaseAsync(a => input.SoCode == a.SoCode && input.SoldCode == a.SoldCode);

            await contractRepository.Value.InsertAsync(contract);
            return "导入成功";
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input">查询组合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<ContractGetListOutput>> GetPageAsync(PageInput<ContractGetListInPut> input)
        {
            var beginDate = input.Filter.CreatedTimeFrom;
            var endDate = input.Filter.CreatedTimeTo?.AddDays(1) ?? input.Filter.CreatedTimeTo;

            var list = await contractRepository.Value.Select
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Key),
                    a => a.DealerName.Contains(input.Filter.Key) || a.SoldCode.Contains(input.Filter.Key))
                .WhereIf(input.Filter.CreatedTimeFrom != null, a => a.CreatedTime >= beginDate)
                .WhereIf(input.Filter.CreatedTimeTo != null, a => a.CreatedTime <= endDate)
                .Count(out var total)
                .OrderByDescending(a => a.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync(a => new ContractGetListOutput()
                {
                    Id = a.Id,
                    DealerName = a.DealerName,
                    SoCode = a.SoCode,
                    SoldCode = a.SoldCode,
                    FileId = a.FileId,
                    FileName = a.File.FileName,
                    CreatedTime = a.CreatedTime,
                    CreatedUserName = a.CreatedUserName,
                    CreatedUserRealName = a.CreatedUserRealName,
                    ModifiedTime = a.ModifiedTime,
                    ModifiedUserName = a.ModifiedUserName,
                    ModifiedUserRealName = a.ModifiedUserRealName,
                });

            var data = new PageOutput<ContractGetListOutput>()
            {
                List = list,
                Total = total
            };

            return data;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            await contractRepository.Value.SoftDeleteAsync(id);
        }
    }
}
