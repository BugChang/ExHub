using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npoi.Mapper;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Domain.RebatePool;
using ZhonTai.Admin.Services.Dealer;
using ZhonTai.Admin.Services.Order.Dto;
using ZhonTai.Admin.Services.Product;
using ZhonTai.Admin.Services.RebatePool.Dto;
using ZhonTai.Admin.Services.User.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.RebatePool
{
    /// <summary>
    /// 返利池服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class RebatePoolService : BaseService, IRebatePoolService, IDynamicApi
    {

        private readonly Lazy<IRebatePoolRepository> _rebatePoolRep;
        private readonly Lazy<IDealerService> _dealerService;
        private readonly Lazy<IProductService> _productService;

        public RebatePoolService(
            Lazy<IRebatePoolRepository> rebatePoolRep,
            Lazy<IDealerService> dealerService,
           Lazy<IProductService> productService)
        {
            _rebatePoolRep = rebatePoolRep;
            _dealerService = dealerService;
            _productService = productService;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="pageInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<RebateGetPageOutput>> GetPageAsync(PageInput<RebateGetPageInput> pageInput)
        {
            var list = await _rebatePoolRep.Value.Orm.Select<RebatePoolEntity, DealerEntity, ProductEntity>()
                 .InnerJoin((rebatePool, dealer, product) => rebatePool.ProductCode == product.Code)
                 .InnerJoin((rebatePool, dealer, product) => rebatePool.SoldCode == dealer.SoldCode)
                 .WhereIf(!string.IsNullOrEmpty(pageInput.Filter?.DealerKey), a =>
                     a.t2.SoldCode.Contains(pageInput.Filter.DealerKey) ||
                     a.t2.Name.Contains(pageInput.Filter.DealerKey))
                 .WhereIf(!string.IsNullOrEmpty(pageInput.Filter?.ProductKey),
                     a =>
                         a.t3.Code.Contains(pageInput.Filter.ProductKey) ||
                         a.t3.Name.Contains(pageInput.Filter.ProductKey))
                 .Count(out var total)
                 .Page(pageInput.CurrentPage, pageInput.PageSize)
                 .ToListAsync(a => new RebateGetPageOutput
                 {
                     DealerName = a.t2.Name,
                     ProductName = a.t3.Name
                 });
            var data = new PageOutput<RebateGetPageOutput>()
            {
                List = list,
                Total = total
            };
            return data;

        }

        /// <summary>
        /// 查询单经销商单产品返利池
        /// </summary>
        /// <param name="soldCode"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public async Task<RebateGetPageOutput> GetAsync(string soldCode, string productCode)
        {
            var rebatePool = await _rebatePoolRep.Value.Select
                .Where(p => p.SoldCode == soldCode && p.ProductCode == productCode)
                .ToOneAsync();

            var data = new RebateGetPageOutput
            {
                SoldCode = soldCode,
                ProductCode = productCode,
                Amount = rebatePool?.Amount ?? 0
            };
            return data;

        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<List<RebateUploadOutput>> ImportAsync(IFormFile formFile)
        {
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper.Map<RebateUploadInput>("Customer Number", a => a.SoldCode)
                .Map<RebateUploadInput>("Material Number", a => a.ProductCode)
                .Map<RebateUploadInput>("Rebate Amount", a => a.Amount)
                .Map<RebateUploadInput>("Last Updated date", a => a.LastUpdateTime)
                .Map<RebateUploadInput>("Last Uploaded date", a => a.LastUploadedTime);

            var rebates = mapper.Take<RebateUploadInput>().Select(i => i.Value).ToList();
            var output = new List<RebateUploadOutput>();
            int rowId = 2;

            if (!rebates.Any())
            {
                output.Add(new RebateUploadOutput { RowId = 0, ErrorMsg = "未检测到数据" });
            }
            // 获取全部经销商和产品
            var dealers = await _dealerService.Value.GetListAsync();
            var products = await _productService.Value.GetListAsync();

            foreach (var rebate in rebates)
            {
                if (string.IsNullOrWhiteSpace(rebate.SoldCode))
                {
                    output.Add(new RebateUploadOutput { RowId = rowId, ErrorMsg = "经销商代码不可为空" });
                }
                else if (dealers.All(p => p.SoldCode != rebate.SoldCode))
                {
                    output.Add(new RebateUploadOutput { RowId = rowId, ErrorMsg = $"该经销商不存在！" });
                }
                if (string.IsNullOrWhiteSpace(rebate.ProductCode))
                {
                    output.Add(new RebateUploadOutput { RowId = rowId, ErrorMsg = "产品代码不可为空" });
                }
                else if (products.All(p => p.Code != rebate.ProductCode))
                {
                    output.Add(new RebateUploadOutput { RowId = rowId, ErrorMsg = $"该产品不存在！" });
                }
                rowId++;
            }

            // 有错误数据则直接返回
            if (output.Any())
            {
                return output;
            }

            await _rebatePoolRep.Value.DeleteAsync(a => true);
            await _rebatePoolRep.Value.InsertAsync(rebates.Select(p => new RebatePoolEntity
            {
                SoldCode = p.SoldCode,
                ProductCode = p.ProductCode,
                Amount = p.Amount,
                RebateUpdateTime = p.LastUpdateTime ?? p.LastUploadedTime
            }).ToList());

            return output;
        }
    }
}
