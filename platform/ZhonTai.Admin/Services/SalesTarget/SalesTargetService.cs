using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Db;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Domain.Quota;
using ZhonTai.Admin.Domain.SalesTarget;
using ZhonTai.Admin.Services.Quota.Dto;
using ZhonTai.Admin.Services.SalesTarget.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.SalesTarget
{
    /// <summary>
    /// 销售指标模块
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class SalesTargetService : BaseService, IDynamicApi
    {
        private readonly Lazy<ISalesTargetRepository> _salesTargetRep;
        private readonly Lazy<IProductRepository> _productRep;
        private readonly Lazy<IQuotaApprovalRepository> _quotaApprovalRep;

        public SalesTargetService(Lazy<ISalesTargetRepository> salesTargetRep, Lazy<IProductRepository> productRep, Lazy<IQuotaApprovalRepository> quotaApprovalRep)
        {
            _salesTargetRep = salesTargetRep;
            _productRep = productRep;
            _quotaApprovalRep = quotaApprovalRep;
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<long> AddAsync(SalesTargetAddInput input)
        {
            if (input.EffectiveDate.Date >= input.ExpirationDate.Date)
            {
                throw ResultOutput.Exception($"生效日期不能晚于或等于失效日期");
            }

            var duplicates = input.Items
                .GroupBy(item => item.ProductCode)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
            if (duplicates.Count > 0)
            {
                throw ResultOutput.Exception($"不允许出现重复项：{string.Join(",", duplicates)}");
            }

            if (input.Items.Sum(a => a.Count) <= 0)
            {
                throw ResultOutput.Exception($"销售指标不能小于等于0");
            }

            // 校验产品有效性
            foreach (var item in input.Items)
            {
                var product = await _productRep.Value.GetAsync(a => a.Code == item.ProductCode);
                if (product == null)
                {
                    throw ResultOutput.Exception($"产品不存在：{item.ProductCode}");
                }
            }

            var existEntity = await _salesTargetRep.Value.Select.Where(a =>
                  a.Status == SalesTargetStatus.Active &&
                  a.EffectiveDate.Date == input.EffectiveDate.Date &&
                  a.ExpirationDate.Date == input.ExpirationDate.Date)
                  .ToOneAsync();

            if (existEntity != null && existEntity.EffectiveDate.Date == input.EffectiveDate.Date && existEntity.ExpirationDate.Date == input.ExpirationDate.Date)
            {
                existEntity.Status = SalesTargetStatus.InActive;
                await _salesTargetRep.Value.UpdateAsync(existEntity);
            }
            else
            {
                var dbLastEntity = await _salesTargetRep.Value.Select.Where(a => a.Status == SalesTargetStatus.Active).OrderByDescending(a => a.ExpirationDate).ToOneAsync();
                if (dbLastEntity != null && dbLastEntity.ExpirationDate.Date != input.EffectiveDate.Date)
                {
                    throw ResultOutput.Exception($"生效日期必须从前一个失效日期开始");
                }
            }

            _salesTargetRep.Value.DbContextOptions.EnableCascadeSave = true;
            var entity = Mapper.Map<SalesTargetEntity>(input);
            var quotaCount = await _quotaApprovalRep.Value.Select.Where(a =>
                 a.IsLatest &&
                 a.EffectiveDate.Date == entity.EffectiveDate.Date &&
                 a.ExpirationDate.Date == entity.ExpirationDate.Date).SumAsync(a => a.TotalCount);
            if (quotaCount > entity.Items.Sum(a => a.Count))
            {
                entity.HasWarning = true;
            }
            await _salesTargetRep.Value.InsertAsync(entity);
            return entity.Id;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<SalesTargetGetPageOutput>> GetPageAsync(PageInput<SalesTargetGetPageInput> input)
        {

            var list = await _salesTargetRep.Value.Select
                .IncludeMany(a => a.Items)
                .Include(a => a.File)
                .WhereIf(!string.IsNullOrEmpty(input.Filter.YearMonth), a => a.YearMonth == input.Filter.YearMonth)
                .WhereIf(input.Filter.Status is EffectiveStatus.InActive, a => a.Status == SalesTargetStatus.InActive)
                .WhereIf(input.Filter.Status is EffectiveStatus.NotActive, a => a.Status == SalesTargetStatus.Active && DateTime.Now < a.EffectiveDate)
                .WhereIf(input.Filter.Status is EffectiveStatus.Active, a => a.Status == SalesTargetStatus.Active && DateTime.Now >= a.EffectiveDate && DateTime.Now <= a.ExpirationDate)
                .Count(out long total)
                .OrderBy(a => a.Status)
                .OrderByDescending(a => a.EffectiveDate)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync();
            var data = new PageOutput<SalesTargetGetPageOutput>()
            {
                List = Mapper.Map<List<SalesTargetGetPageOutput>>(list),
                Total = total
            };
            return data;
        }

        /// <summary>
        /// 检查 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> CheckAsync(long id)
        {
            var salesTarget = await _salesTargetRep.Value.Select.IncludeMany(a => a.Items).Where(a => a.Id == id).FirstAsync();
            var quotaCount = await _quotaApprovalRep.Value.Select.Where(a =>
                a.IsLatest &&
                a.EffectiveDate.Date == salesTarget.EffectiveDate.Date &&
                a.ExpirationDate.Date == salesTarget.ExpirationDate.Date).SumAsync(a => a.TotalCount);

            salesTarget.HasWarning = quotaCount > salesTarget.Items.Sum(a => a.Count);
            await _salesTargetRep.Value.InsertOrUpdateAsync(salesTarget);
            return salesTarget.HasWarning;
        }
    }
}
