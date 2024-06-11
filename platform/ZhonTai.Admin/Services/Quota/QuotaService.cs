using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogicExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Npoi.Mapper;
using NPOI.SS.Formula.Functions;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.File;
using ZhonTai.Admin.Domain.OrderDemand;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Domain.Quota;
using ZhonTai.Admin.Domain.SalesTarget;
using ZhonTai.Admin.Services.Dealer;
using ZhonTai.Admin.Services.File;
using ZhonTai.Admin.Services.OrderDemand.Dto;
using ZhonTai.Admin.Services.Product;
using ZhonTai.Admin.Services.Quota.Dto;
using ZhonTai.Admin.Services.User;
using ZhonTai.Common.Extensions;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using Mapper = Npoi.Mapper.Mapper;

namespace ZhonTai.Admin.Services.Quota
{
    /// <summary>
    /// 配额服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class QuotaService(Lazy<IQuotaRepository> quotaRep,
            Lazy<IDealerService> dealerService,
            Lazy<IProductService> productService,
            Lazy<ISalesTargetRepository> salesTargetRep,
            Lazy<IUserService> userService,
            Lazy<IFileService> fileService,
            Lazy<IQuotaApprovalRepository> quotaApprovalRep, Lazy<IOrderDemandRepository> orderDemandRep,
            Lazy<IProductRepository> productRep)
        : BaseService, IQuotaService, IDynamicApi
    {
        /// <summary>
        /// 查询剩余配额数
        /// </summary>
        /// <param name="soldCode">经销商代码</param>
        /// <param name="productCode">产品代码</param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(string soldCode, string productCode)
        {
            var quotaList = await quotaRep.Value.Select.Where(
                    a => a.SoldCode == soldCode
                         && a.ProductCode == productCode
                         && a.Status == QuotaStatus.Approved
                         && a.EffectiveDate.Date <= DateTime.Now.Date
                         && a.ExpirationDate.Date > DateTime.Now.Date)
                .ToListAsync();

            if (!quotaList.Any())
                return 0;

            var count = await orderDemandRep.Value.Orm.Select<OrderDemandEntity, OrderDemandItemEntity>()
                  .InnerJoin((a, b) => a.Id == b.OrderDemandId && b.ProductCode == productCode)
                  .Where((a, b) =>
                    a.Status != OrderDemandStatus.Deleted &&
                    a.Status != OrderDemandStatus.Saved &&
                    a.SoldCode == soldCode &&
                    a.FirstCommitTime >= quotaList.First().EffectiveDate &&
                    a.FirstCommitTime < quotaList.First().ExpirationDate)
                  .SumAsync((a, b) => b.NeedCount);


            return quotaList.Sum(q => q.Count) - (int)count;
        }


        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<QuotaGetPageOutput>> GetPageAsync(PageInput<QuotaGetPageInput> input)
        {
            var list = await quotaRep.Value.Orm.Select<QuotaEntity, DealerEntity, ProductEntity>()
                  .LeftJoin((quota, dealer, product) => quota.SoldCode == dealer.SoldCode)
                  .LeftJoin((quota, dealer, product) => quota.ProductCode == product.Code)
                  .Where((quota, dealer, product) => quota.Status == QuotaStatus.Approved && quota.Count != 0)
                  .WhereIf(!string.IsNullOrEmpty(input.Filter.ProductCode), a => a.t1.ProductCode == input.Filter.ProductCode)
                  .WhereIf(!string.IsNullOrEmpty(input.Filter.DealerKey), a => a.t2.Name.Contains(input.Filter.DealerKey) || a.t2.SoldCode.Contains(input.Filter.DealerKey))
                  .WhereIf(!string.IsNullOrEmpty(input.Filter.YearMonth), a => a.t1.YearMonth == input.Filter.YearMonth)
                  .WhereIf(input.Filter.EffectiveStatus == EffectiveStatus.Active, a => a.t1.EffectiveDate <= DateTime.Today && a.t1.ExpirationDate > DateTime.Today)
                  .WhereIf(input.Filter.EffectiveStatus == EffectiveStatus.InActive, a => a.t1.ExpirationDate <= DateTime.Today)
                  .WhereIf(input.Filter.EffectiveStatus == EffectiveStatus.NotActive, a => a.t1.EffectiveDate > DateTime.Today)
                  .GroupBy((quota, dealer, product) => new
                  {
                      quota.SoldCode,
                      DealerName = dealer.Name,
                      dealer.Region,
                      quota.ProductCode,
                      ProductName = product.Name,
                      quota.EffectiveDate,
                      quota.ExpirationDate,
                      quota.CreatedUserRealName,
                      quota.CreatedUserName
                  })
                  .Count(out var total)
                  .Page(input.CurrentPage, input.PageSize)
                  .OrderByDescending(a => a.Key.EffectiveDate)
                  .ToListAsync(a => new
                      QuotaGetPageOutput()
                  {
                      HasChild = a.Count() > 1 ? true : false,
                      Count = (int)a.Sum(a.Value.Item1.Count),
                      DealerName = a.Key.DealerName,
                      SoldCode = a.Key.SoldCode,
                      ProductCode = a.Key.ProductCode,
                      ProductName = a.Key.ProductName,
                      CreatedUserRealName = a.Key.CreatedUserRealName,
                      Region = a.Value.Item2.Region,
                      EffectiveDate = a.Key.EffectiveDate,
                      ExpirationDate = a.Key.ExpirationDate,
                      CreatedUserName = a.Key.CreatedUserName,
                      CreatedTime = a.Max(a.Value.Item1.CreatedTime),
                      ModifiedTime = a.Max(a.Value.Item1.ModifiedTime),
                      YearMonth = a.Max(a.Value.Item1.YearMonth)
                  });

            foreach (var quota in list)
            {
                // todo:看情况进行性能优化
                var orderDemands = await orderDemandRep.Value.Select
                    .IncludeMany(a => a.Items)
                    .Where(a =>
                        a.Status != OrderDemandStatus.Deleted &&
                        a.SoldCode == quota.SoldCode &&
                        a.FirstCommitTime >= quota.EffectiveDate &&
                        a.FirstCommitTime < quota.ExpirationDate)
                    .ToListAsync();
                var count = orderDemands.Sum(a =>
                    a.Items.Where(c => c.ProductCode == quota.ProductCode).Sum(i => i.NeedCount));

                quota.RemainingCount = quota.Count - count;
                quota.RowId = Guid.NewGuid().ToString("N");
            }
            var data = new PageOutput<QuotaGetPageOutput>()
            {
                List = list,
                Total = total
            };
            return data;
        }


        /// <summary>
        /// 查询审批记录分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<QuotaGetApproveLogPageOutput>> GetApproveLogPageAsync(PageInput<QuotaGetApproveLogPageInput> input)
        {
            var list = await quotaRep.Value.Orm.Select<QuotaEntity, DealerEntity, ProductEntity>()
                  .LeftJoin((quota, dealer, product) => quota.SoldCode == dealer.SoldCode)
                  .LeftJoin((quota, dealer, product) => quota.ProductCode == product.Code)
                  .Where((quota, dealer, product) => quota.Status != QuotaStatus.InApprove)
                  .WhereIf(!string.IsNullOrEmpty(input.Filter.DealerKey), a => a.t2.Name.Contains(input.Filter.DealerKey) || a.t2.SoldCode.Contains(input.Filter.DealerKey))
                  .WhereIf(!string.IsNullOrEmpty(input.Filter.YearMonth), a => a.t1.YearMonth == input.Filter.YearMonth)
                  .WhereIf(input.Filter.OperateTimeFrom != null, a => a.t1.ModifiedTime >= input.Filter.OperateTimeFrom)
                  .WhereIf(input.Filter.OperateTimeTo != null, a => a.t1.ModifiedTime < input.Filter.OperateTimeTo.Value.AddDays(1))
                  .Count(out var total)
                  .Page(input.CurrentPage, input.PageSize)
                  .OrderByDescending(a => a.t1.Id)
                  .ToListAsync(a => new
                      QuotaGetApproveLogPageOutput()
                  {
                      DealerType = a.t2.Type,
                      SoldCode = a.t2.SoldCode,
                      DealerName = a.t2.Name,
                      ProductCode = a.t3.Code,
                      ProductName = a.t3.Name,
                      TotalChange = a.t1.TotalChange,
                      PendingCount = a.t1.Count,
                      Status = a.t1.Status,
                      EffectiveDate = a.t1.EffectiveDate,
                      ExpirationDate = a.t1.ExpirationDate,
                      YearMonth = a.t1.YearMonth,
                      ModifiedTime = a.t1.ModifiedTime,
                      ModifiedUserName = a.t1.ModifiedUserName,
                      ModifiedUserRealName = a.t1.ModifiedUserRealName
                  });

            var data = new PageOutput<QuotaGetApproveLogPageOutput>()
            {
                List = list,
                Total = total
            };
            return data;
        }

        /// <summary>
        /// 查询配额子数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<QuotaGetChildListOutput>> GetChildListAsync(QuotaGetChildListInput input)
        {
            return await quotaRep.Value.Select.Where(a =>
                    a.Status == QuotaStatus.Approved
                    && a.TotalChange != 0
                    && a.SoldCode == input.SoldCode
                    && a.ProductCode == input.ProductCode
                    && a.EffectiveDate.Date == input.EffectiveDate.Date
                    && a.ExpirationDate.Date == input.ExpirationDate.Date
                    && a.CreatedUserName == input.CreatedUserName)
                .ToListAsync<QuotaGetChildListOutput>();
        }

        /// <summary>
        /// 配额附件检查
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<QuotaCheckOutput>> CheckAsync(IFormFile formFile)
        {
            // 检查文件格式
            if (formFile == null || !formFile.FileName.EndsWith(".xlsx"))
            {
                throw ResultOutput.Exception("Please upload Excel file!");
            }
            // 读取Excel字段
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper
                .Map<QuotaUploadInput>("区域", a => a.Region)
                .Map<QuotaUploadInput>("经销商代码", a => a.SoldCode)
                .Map<QuotaUploadInput>("经销商名称", a => a.DealerName)
                .Map<QuotaUploadInput>("产品代码", a => a.ProductCode)
                .Map<QuotaUploadInput>("产品名称", a => a.ProductName)
                .Map<QuotaUploadInput>("配额总数调整（减少请填负数）", a => a.TotalChange)
                .Map<QuotaUploadInput>("待审批配额（减少请填负数）", a => a.PendingCount)
                .Map<QuotaUploadInput>("生效日期（YYYY-MM-DD）", a => a.EffectiveDate)
                .Map<QuotaUploadInput>("失效日期（YYYY-MM-DD）", a => a.ExpirationDate)
                .Map<QuotaUploadInput>("所属年月（YYYY-MM）", a => a.YearMonth);

            var quotas = mapper.Take<QuotaUploadInput>().Select(i => i.Value).ToList();
            if (!quotas.Any())
            {
                throw ResultOutput.Exception("文件内容不能为空！");
            }

            var repeat = quotas.GroupBy(a => new { a.SoldCode, a.ProductCode, a.EffectiveDate, a.ExpirationDate })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key).ToList();
            if (repeat.Any())
            {
                throw ResultOutput.Exception("同区间、同经销商、同产品下不允许出现多条数据");
            }

            // 获取待查询的经销商和产品

            var allDealers = await dealerService.Value.GetListAsync();
            var allProducts = await productService.Value.GetListAsync();
            var allSalesTargets = await salesTargetRep.Value.Select.IncludeMany(a => a.Items).Where(a => a.Status == SalesTargetStatus.Active).ToListAsync();

            var output = new List<QuotaCheckOutput>();
            var rowId = 2;
            foreach (var quota in quotas)
            {
                if (quota.TotalChange == 0 && quota.PendingCount == 0)
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = $"配额总数调整、待审批配额不能都为0" });
                }

                if (quota.YearMonth.IsNullOrEmpty())
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = $"所属年月必填！" });
                }
                else if (!DateTime.TryParseExact(quota.YearMonth, "yyyy-MM", null, DateTimeStyles.None, out _))
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = $"所属年月格式错误！" });
                }

                // 校验经销商姓名和代码是否匹配
                var dealer = allDealers.FirstOrDefault(p => p.SoldCode == quota.SoldCode && p.Name == quota.DealerName);
                if (dealer == null)
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = $"经销商代码校验失败！" });
                }
                // 校验产品代码和产品名称是否匹配
                var product = allProducts.FirstOrDefault(p => p.Code == quota.ProductCode);
                if (product == null || product.Name != quota.ProductName)
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = $"产品代码校验失败！" });
                }
                // 校验生效日期和失效日期是否存在销售指标
                var salesTarget = allSalesTargets.FirstOrDefault(p => p.EffectiveDate == quota.EffectiveDate && p.ExpirationDate == quota.ExpirationDate);
                if (salesTarget == null)
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = $"生效时间和结束时间与销售指标不符！" });
                }
                else
                {
                    // 校验配额总数不能大于销售指标
                    var totalQuota = await quotaApprovalRep.Value.Select.Where(a =>
                        a.IsLatest &&
                        a.EffectiveDate.Date == salesTarget.EffectiveDate.Date &&
                        a.ExpirationDate.Date == salesTarget.ExpirationDate.Date).SumAsync(a => a.TotalCount);
                    var totalChange = quotas.Where(a => a.EffectiveDate.Date == salesTarget.EffectiveDate.Date
                                       && a.ExpirationDate.Date == salesTarget.ExpirationDate.Date)
                         .Sum(a => a.TotalChange);
                    if (totalQuota + totalChange > salesTarget.Items.Sum(a => a.Count))
                    {
                        output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = "超出销售指标总数" });
                    }
                }

                // 校验产品审批配额不能大于总配额
                var lastApprove = await quotaApprovalRep.Value
                    .Select.IncludeMany(a => a.Quotas)
                    .Where(a => a.IsLatest &&
                                a.EffectiveDate.Date == salesTarget.EffectiveDate.Date &&
                                a.ExpirationDate.Date == salesTarget.ExpirationDate.Date &&
                                a.SoldCode == quota.SoldCode).ToOneAsync();

                var lastQuota = lastApprove?.Quotas?.FirstOrDefault(a =>
                   a.SoldCode == quota.SoldCode && a.ProductCode == quota.ProductCode);

                if (quota.PendingCount + (lastQuota?.Count ?? 0) > (lastQuota?.TotalCount ?? 0) + quota.TotalChange)
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = "超出当月配额总数" });
                }

                if ((lastQuota?.TotalCount ?? 0) + quota.TotalChange < 0)
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = "调整后配额总数不能小于0" });
                }

                // 校验PR总数量
                var prCount = await orderDemandRep.Value.Select
                    .IncludeMany(a => a.Items)
                    .Where(a => a.SoldCode == quota.SoldCode
                                && a.Status != OrderDemandStatus.Deleted
                                && a.Status != OrderDemandStatus.Saved
                                && a.FirstCommitTime >= quota.EffectiveDate.Date
                                && a.FirstCommitTime < quota.ExpirationDate.Date)
                    .ToListAsync(a => a.Items.Sum(i => i.NeedCount));


                if (prCount.Sum() > quota.PendingCount + (lastQuota?.Count ?? 0))
                {
                    output.Add(new QuotaCheckOutput { RowId = rowId, ErrorMsg = "已提交PR超出配额" });
                }

                rowId++;
            }
            return output;
        }

        /// <summary>
        /// 创建审批
        /// </summary>
        /// <param name="quotaFile"></param>
        /// <param name="approveFile"></param>
        /// <returns></returns>
        public async Task<List<QuotaCheckOutput>> CreateApproveAsync(IFormFile quotaFile, IFormFile approveFile = null)
        {
            var output = await CheckAsync(quotaFile);
            if (output.Any())
            {
                return output;
            }
            // 读取Excel字段
            var mapper = new Mapper(quotaFile.OpenReadStream());
            mapper
                .Map<QuotaUploadInput>("区域", a => a.Region)
                .Map<QuotaUploadInput>("经销商代码", a => a.SoldCode)
                .Map<QuotaUploadInput>("经销商名称", a => a.DealerName)
                .Map<QuotaUploadInput>("产品代码", a => a.ProductCode)
                .Map<QuotaUploadInput>("产品名称", a => a.ProductName)
                .Map<QuotaUploadInput>("配额总数调整（减少请填负数）", a => a.TotalChange)
                .Map<QuotaUploadInput>("待审批配额（减少请填负数）", a => a.PendingCount)
                .Map<QuotaUploadInput>("生效日期（YYYY-MM-DD）", a => a.EffectiveDate)
                .Map<QuotaUploadInput>("失效日期（YYYY-MM-DD）", a => a.ExpirationDate)
                .Map<QuotaUploadInput>("所属年月（YYYY-MM）", a => a.YearMonth);
            var quotaApproves = mapper.Take<QuotaUploadInput>().Select(i => i.Value).ToList();



            // 按经销商、生效区间内组合审批数据
            var newApproves = quotaApproves.GroupBy(a => new { a.SoldCode, a.EffectiveDate, a.ExpirationDate })
                .Select(a => new QuotaApprovalEntity
                {
                    SoldCode = a.Key.SoldCode,
                    EffectiveDate = a.Key.EffectiveDate,
                    ExpirationDate = a.Key.ExpirationDate,
                    TotalChange = a.Sum(c => c.TotalChange),
                    PendingCount = a.Sum(c => c.PendingCount),
                    Region = a.Max(c => c.Region),
                    IsLatest = true,
                    YearMonth = a.Max(c => c.YearMonth),
                    Status = QuotaStatus.InApprove,
                    Quotas = new List<QuotaEntity>()
                }).ToList();

            // 查询DB已有审批数据
            var dbApprovesQuery = quotaApprovalRep.Value.Select
                .IncludeMany(a => a.Quotas)
                .Where(a => a.IsLatest && newApproves.Any(b =>
                    b.SoldCode == a.SoldCode && b.EffectiveDate.Date == a.EffectiveDate.Date &&
                    b.ExpirationDate.Date == a.ExpirationDate.Date));

            var dbApproves = await dbApprovesQuery.ToListAsync();
            if (dbApproves.Any(a => a.Status == QuotaStatus.InApprove))
            {
                throw ResultOutput.Exception("同一经销商同生效区间内只应该存在一条待审批");
            }
            // 上传文件
            var file = await fileService.Value.UploadFileAsync(quotaFile);
            var approveFileEntity = approveFile is { Length: > 0 } ? await fileService.Value.UploadFileAsync(approveFile) : new FileEntity();

            var allProducts = await productService.Value.GetListAsync();

            foreach (var newApprove in newApproves)
            {
                var dbApprove = dbApproves.FirstOrDefault(a =>
                    a.SoldCode == newApprove.SoldCode &&
                    a.EffectiveDate.Date == newApprove.EffectiveDate.Date &&
                    a.ExpirationDate.Date == newApprove.ExpirationDate.Date);

                newApprove.FileId = file.Id;
                newApprove.ApproveFileId = approveFileEntity.Id;
                newApprove.TotalCount = dbApprove?.TotalCount ?? 0;
                newApprove.ApprovedCount = dbApprove?.ApprovedCount ?? 0;

                // 获取该条审批对应的明细数据

                var quotas = quotaApproves.Where(a =>
                    a.SoldCode == newApprove.SoldCode &&
                    a.EffectiveDate.Date == newApprove.EffectiveDate.Date &&
                    a.ExpirationDate.Date == newApprove.ExpirationDate.Date).ToList();

                foreach (var product in allProducts)
                {
                    var dbQuota = dbApprove?.Quotas.FirstOrDefault(a => a.ProductCode == product.Code);
                    var newQuota = quotas.FirstOrDefault(a => a.ProductCode == product.Code);
                    if (newQuota != null)
                    {
                        var quotaEntity = new QuotaEntity()
                        {
                            ApprovedCount = dbQuota?.ApprovedCount ?? 0,
                            Count = newQuota.PendingCount,
                            TotalCount = dbQuota?.TotalCount ?? 0,
                            TotalChange = newQuota.TotalChange,
                            SoldCode = newQuota.SoldCode,
                            ProductCode = newQuota.ProductCode,
                            EffectiveDate = newQuota.EffectiveDate.Date,
                            ExpirationDate = newQuota.ExpirationDate.Date,
                            YearMonth = newQuota.YearMonth,
                            Status = QuotaStatus.InApprove,
                            Region = newQuota.Region
                        };
                        newApprove.Quotas.Add(quotaEntity);
                    }
                    else if (dbQuota != null)
                    {
                        newApprove.Quotas.Add(new QuotaEntity()
                        {
                            ApprovedCount = dbQuota.ApprovedCount,
                            Count = 0,
                            TotalCount = dbQuota.TotalCount,
                            TotalChange = 0,
                            SoldCode = dbQuota.SoldCode,
                            ProductCode = dbQuota.ProductCode,
                            EffectiveDate = dbQuota.EffectiveDate.Date,
                            ExpirationDate = dbQuota.ExpirationDate.Date,
                            YearMonth = dbQuota.YearMonth,
                            Status = QuotaStatus.InApprove,
                            Region = dbQuota.Region,
                        });
                    }
                }
            }

            await dbApprovesQuery.ToUpdate().Set(a => a.IsLatest, false).ExecuteAffrowsAsync();
            quotaApprovalRep.Value.DbContextOptions.EnableCascadeSave = true;
            await quotaApprovalRep.Value.InsertAsync(newApproves);

            return output;
        }

        /// <summary>
        /// 审批查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<QuotaGetApprovalPageOutput>> GetApprovalPage(PageInput<QuotaGetApprovalPageInput> input)
        {
            var result = await quotaApprovalRep.Value.Select
                .IncludeMany(a => a.Quotas)
                .Where(a => a.IsLatest && (a.TotalCount > 0 || a.Status == QuotaStatus.InApprove))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.DealerKey), a => a.SoldCode.Contains(input.Filter.DealerKey) || a.Dealer.Name.Contains(input.Filter.DealerKey))
                .WhereIf(input.Filter.DealerType != null, a => a.Dealer.Type == input.Filter.DealerType)
                .WhereIf(!input.Filter.YearMonth.IsNullOrEmpty(), a => a.YearMonth == input.Filter.YearMonth)
                .WhereIf(input.Filter.Status == ApproveStatus.Approved, a => a.Status != QuotaStatus.InApprove)
                .WhereIf(input.Filter.Status == ApproveStatus.InApprove, a => a.Status == QuotaStatus.InApprove)
                .Count(out var total)
                .Page(input.CurrentPage, input.PageSize)
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync(p => new QuotaGetApprovalPageOutput
                {
                    Id = p.Id,
                    Region = p.Region,
                    SoldCode = p.SoldCode,
                    DealerType = p.Dealer.Type,
                    DealerName = p.Dealer.Name,
                    TotalCount = p.TotalCount,
                    TotalChange = p.Status == QuotaStatus.InApprove ? p.TotalChange : 0,
                    PendingCount = p.Status == QuotaStatus.InApprove ? p.PendingCount : 0,
                    ApprovedCount = p.ApprovedCount,
                    Status = p.Status == QuotaStatus.InApprove ? ApproveStatus.InApprove : ApproveStatus.Approved,
                    EffectiveDate = p.EffectiveDate,
                    ExpirationDate = p.ExpirationDate,
                    YearMonth = p.YearMonth,
                    CreatedUserName = p.CreatedUserName,
                    CreatedTime = p.CreatedTime,
                    PeApproveUserName = p.PeApproveUserName,
                    BizApproveUserName = p.BizApproveUserName,
                    AllowWithdraw = (p.CreatedUserId == quotaApprovalRep.Value.User.Id && p.Status == QuotaStatus.InApprove) ? true : false
                });

            foreach (var quota in result)
            {
                var orderDemands = await orderDemandRep.Value.Select
                    .IncludeMany(a => a.Items)
                    .Where(a =>
                        a.Status != OrderDemandStatus.Deleted &&
                        a.SoldCode == quota.SoldCode &&
                        a.FirstCommitTime >= quota.EffectiveDate &&
                        a.FirstCommitTime < quota.ExpirationDate)
                    .ToListAsync(a =>
                        a.Items.Sum(i => i.NeedCount));
                quota.PrCount = quota.ApprovedCount - orderDemands.Sum();
            }

            var pageOutput = new PageOutput<QuotaGetApprovalPageOutput>
            {
                Total = total,
                List = result
            };

            return pageOutput;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="approveIds"></param>
        /// <returns></returns>
        public async Task WithdrawAsync(List<long> approveIds)
        {
            // 修改审批状态为已撤回
            var approves = await quotaApprovalRep.Value.Select
                .Where(p => approveIds.Contains(p.Id)).ToListAsync();

            // 只有自己能撤回自己提交的单子
            var user = quotaRep.Value.User;
            foreach (var approve in approves)
            {
                if (approve.CreatedUserName != user.UserName)
                {
                    throw ResultOutput.Exception("非本人无权限撤回");
                }

                approve.Status = QuotaStatus.Withdraw;

                // 修改配额项状态为已撤回
                var quotas = await quotaRep.Value.Select.Where(p => p.ApprovedId == approve.Id).ToListAsync();
                quotas.ForEach(p => p.Status = QuotaStatus.Withdraw);

                await quotaApprovalRep.Value.UpdateAsync(approve);
                await quotaRep.Value.UpdateAsync(quotas);
            }

        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="approveIds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task ApproveAsync(List<long> approveIds)
        {
            var user = quotaRep.Value.User;

            var currentUser = await userService.Value.GetAsync(quotaRep.Value.User.Id);
            var isBizManager = currentUser.Roles.Count(a => a.Name == "商务总监") > 0;
            var isPeManager = currentUser.Roles.Count(a => a.Name == "PE总监") > 0;
            if (!isBizManager && !isPeManager)
            {
                throw ResultOutput.Exception("无权限操作");
            }

            // 获取审批记录
            var approves = await quotaApprovalRep.Value.Select.Where(p => approveIds.Contains(p.Id)).ToListAsync();
            foreach (var approve in approves)
            {
                if (approve.Status != QuotaStatus.InApprove)
                {
                    throw ResultOutput.Exception("当前状态不允许操作");
                }
                if (isBizManager && string.IsNullOrWhiteSpace(approve.BizApproveUserName))
                {
                    // 第一审批人为空则添加进去，除此之外不做操作
                    approve.BizApproveUserName = user.UserName;
                }
                else if (isPeManager && string.IsNullOrEmpty(approve.PeApproveUserName))
                {
                    // 如果第一审批人不为空，不等于当前用户，且第二审批人为空，则添加进去且设置为已审批
                    approve.PeApproveUserName = user.UserName;
                }
                else
                {
                    throw ResultOutput.Exception("当前角色已完成审批，请勿重复操作！");
                }

                if (!approve.BizApproveUserName.IsNullOrEmpty() && !approve.PeApproveUserName.IsNullOrEmpty())
                {
                    approve.Status = QuotaStatus.Approved;
                    approve.TotalCount += approve.TotalChange;
                    approve.ApprovedCount += approve.PendingCount;
                    // 获取配额项
                    var quotas = await quotaRep.Value.Select.Where(p => p.ApprovedId == approve.Id).ToListAsync();
                    quotas.ForEach(p =>
                    {
                        p.Status = QuotaStatus.Approved;
                        p.TotalCount += p.TotalChange;
                        p.ApprovedCount += p.Count;
                    });
                    await quotaRep.Value.UpdateAsync(quotas);
                }
                await quotaApprovalRep.Value.UpdateAsync(approve);

            }
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="approveIds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task RejectAsync(List<long> approveIds)
        {
            var currentUser = await userService.Value.GetAsync(quotaRep.Value.User.Id);
            var isBizManager = currentUser.Roles.Count(a => a.Name == "商务总监") > 0;
            var isPeManager = currentUser.Roles.Count(a => a.Name == "PE总监") > 0;
            if (!isBizManager && !isPeManager)
            {
                throw ResultOutput.Exception("无权限操作");
            }
            // 修改审批状态为已拒绝
            var approves = await quotaApprovalRep.Value.Select.Where(p => approveIds.Contains(p.Id)).ToListAsync();
            foreach (var approve in approves)
            {
                approve.Status = QuotaStatus.Decline;
                if (isPeManager)
                {
                    approve.PeApproveUserName = currentUser.UserName;
                }
                else
                {
                    approve.BizApproveUserName = currentUser.UserName;
                }
                // 修改配额项状态为已拒绝
                var quotas = await quotaRep.Value.Select.Where(p => p.ApprovedId == approve.Id).ToListAsync();
                quotas.ForEach(p => p.Status = QuotaStatus.Decline);
                await quotaApprovalRep.Value.UpdateAsync(approve);
                await quotaRep.Value.UpdateAsync(quotas);
            }

        }

        /// <summary>
        /// 当月配额概览查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageOutput<MonthQuotaOutput>> MonthQuotatAsync(PageInput<MonthQuotatInput> input)
        {
            if (string.IsNullOrEmpty(input.Filter.YearMonth))
            {
                throw ResultOutput.Exception("请选择所属年月");
            }
            //销售指标
            var sales = await salesTargetRep.Value.Select
               .IncludeMany(a => a.Items)
               .Where(a => a.YearMonth == input.Filter.YearMonth && a.Status == SalesTargetStatus.Active)
               .OrderByDescending(a => a.CreatedTime)
               .ToListAsync();
            //配额审批获取当月配额总数
            var quotaApprovals = await quotaApprovalRep.Value.Select
            .IncludeMany(a => a.Quotas)
            .Where(a => a.YearMonth == input.Filter.YearMonth && a.IsLatest)
            .OrderByDescending(a => a.CreatedTime)
            .ToListAsync();
            Dictionary<string, MonthQuotaDataDto> datas = new Dictionary<string, MonthQuotaDataDto>();
            //List<MonthQuotaDataDto> datas = new List<MonthQuotaDataDto>();
            //销售指标
            foreach (var sale in sales)
            {
                foreach (var item in sale.Items)
                {
                    if (datas.GetValueOrDefault(item.ProductCode) != null)
                    {
                        datas.GetValueOrDefault(item.ProductCode).SaleTotalCount += item.Count;
                    }
                    else
                    {
                        datas.Add(item.ProductCode, new MonthQuotaDataDto
                        {
                            SaleTotalCount = item.Count,
                        });
                    }
                }
            }
            var total = datas.Count;
            datas = datas.Skip((input.CurrentPage - 1) * input.PageSize).Take(input.PageSize).ToDictionary();
            //当月配额总数
            foreach (var quotaApproval in quotaApprovals)
            {
                foreach (var quota in quotaApproval.Quotas)
                {
                    if (datas.GetValueOrDefault(quota.ProductCode) != null)
                    {
                        datas.GetValueOrDefault(quota.ProductCode).QuotaTotalCount += quota.TotalCount;
                    }
                }
            }
            //获取产品名称
            var productCodes = datas.Select(a => a.Key).ToList();
            var products = await productRep.Value.Select
             .Where(a => productCodes.Contains(a.Code))
             .OrderByDescending(a => a.CreatedTime)
             .ToListAsync();
            var res = datas.Select(a => new MonthQuotaOutput
            {
                ProductName = products.FirstOrDefault(b => b.Code == a.Key)?.Name,
                SaleTotalCount = a.Value.SaleTotalCount,
                QuotaTotalCount = a.Value.QuotaTotalCount,
                Difference = a.Value.SaleTotalCount - a.Value.QuotaTotalCount
            }).ToList();

            var pageOutput = new PageOutput<MonthQuotaOutput>
            {
                Total = total,
                List = res
            };

            return pageOutput;
        }

        /// <summary>
        /// 查询配额审批子数据
        /// </summary>
        /// <param name="approveId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<QuotaGetApproveChildListOutput>> GetApproveChildListAsync(long approveId)
        {
            var result = await quotaRep.Value.Select
                .Include(a => a.Product)
                .Where(a =>
                  a.ApprovedId == approveId)
                .ToListAsync(a => new QuotaGetApproveChildListOutput
                {
                    ProductCode = a.ProductCode,
                    ProductName = a.Product.Name,
                    ApprovedCount = a.ApprovedCount,
                    TotalChange = a.Status == QuotaStatus.InApprove ? a.TotalChange : 0,
                    PendingCount = a.Status == QuotaStatus.InApprove ? a.Count : 0,
                });
            return result;
        }


        /// <summary>
        /// 导出配额审批记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileContentResult> ExportAsync(QuotaGetApproveLogPageInput input)
        {
            var quotas = await quotaRep.Value.Orm.Select<QuotaEntity, DealerEntity, ProductEntity>()
                 .LeftJoin((quota, dealer, product) => quota.SoldCode == dealer.SoldCode)
                 .LeftJoin((quota, dealer, product) => quota.ProductCode == product.Code)
                 .Where((quota, dealer, product) => quota.Status != QuotaStatus.InApprove)
                 .WhereIf(!string.IsNullOrEmpty(input.DealerKey), a => a.t2.Name.Contains(input.DealerKey) || a.t2.SoldCode.Contains(input.DealerKey))
                 .WhereIf(!string.IsNullOrEmpty(input.YearMonth), a => a.t1.YearMonth == input.YearMonth)
                 .WhereIf(input.OperateTimeFrom != null, a => a.t1.ModifiedTime >= input.OperateTimeFrom)
                 .WhereIf(input.OperateTimeTo != null, a => a.t1.ModifiedTime < input.OperateTimeTo.Value.AddDays(1))
                 .OrderByDescending(a => a.t1.Id)
                 .ToListAsync(a => new
                     QuotaGetApproveLogPageOutput()
                 {
                     DealerType = a.t2.Type,
                     SoldCode = a.t2.SoldCode,
                     DealerName = a.t2.Name,
                     ProductCode = a.t3.Code,
                     ProductName = a.t3.Name,
                     TotalChange = a.t1.TotalChange,
                     PendingCount = a.t1.Count,
                     Status = a.t1.Status,
                     EffectiveDate = a.t1.EffectiveDate,
                     ExpirationDate = a.t1.ExpirationDate,
                     YearMonth = a.t1.YearMonth,
                     ModifiedTime = a.t1.ModifiedTime,
                     ModifiedUserName = a.t1.ModifiedUserName,
                     ModifiedUserRealName = a.t1.ModifiedUserRealName
                 });

            var list = new List<QuotaApprovalExportOutput>();

            foreach (var quota in quotas)
            {
                var dto = new QuotaApprovalExportOutput
                {
                    DealerName = quota.DealerName,
                    DealerType = quota.DealerTypeDesc,
                    SoldCode = quota.SoldCode,
                    Status = quota.StatusDesc,
                    EffectiveDate = quota.EffectiveDate.ToString(CultureInfo.InvariantCulture),
                    ExpirationDate = quota.ExpirationDate.ToString(CultureInfo.InvariantCulture),
                    ModifiedTime = quota.ModifiedTime.ToString(),
                    ModifiedUserRealName = quota.ModifiedUserRealName,
                    PendingCount = quota.PendingCount.ToString(),
                    ProductCode = quota.ProductCode,
                    ProductName = quota.ProductName,
                    TotalChange = quota.TotalChange.ToString(),
                    YearMonth = quota.YearMonth
                };
                list.Add(dto);
            }

            var mapper = new Mapper();
            MemoryStream stream = new MemoryStream();

            mapper.Save(stream, list, "Sheet1", false);

            return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"配额审批记录_{DateTime.Now:yyyyMMddHHmm}.xlsx"
            };
        }
    }
}
