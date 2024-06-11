using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Npoi.Mapper;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Services.Product.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;
using ZhonTai.Admin.Domain.ProductPrice;
using ZhonTai.Admin.Services.Dealer;
using ZhonTai.Admin.Services.Job;
using ZhonTai.Admin.Services.Quota;
using ZhonTai.Admin.Services.RebatePool;
using ZhonTai.Admin.Services.Dealer.Dto;

namespace ZhonTai.Admin.Services.Product
{

    /// <summary>
    /// 产品服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class ProductService
        (Lazy<IProductRepository> productRep,
            Lazy<IProductPriceRepository> productPriceRep,
            IDealerService dealerService,
            IQuotaService quotaService,
            IRebatePoolService rebatePoolService) : BaseService,
            IProductService,
            IDynamicApi
    {


        /// <summary>
        /// 查询经销商产品列表
        /// </summary>
        /// <param name="soldCode">经销商代码</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public Task<List<ProductListOutput>> GetDealerListAsync(string soldCode, string key)
        {
            return productRep.Value.Orm.Select<ProductEntity, ProductPriceEntity>()
                .InnerJoin((a, b) => a.Code == b.ProductCode)
                .WhereIf(!string.IsNullOrEmpty(key), a => a.t1.Code.Contains(key))
                .Where(a => a.t2.SoldCode == soldCode)
                .ToListAsync<ProductListOutput>();
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductListOutput>> GetListAsync()
        {
            var products = await Cache.GetOrSetAsync(CacheKeys.Products,
                () => productRep.Value.Select.ToListAsync<ProductListOutput>());
            return products;
        }

        /// <summary>
        /// 查询聚合
        /// </summary>
        /// <param name="code">产品代码</param>
        /// <param name="soldCode">经销商代码</param>
        /// <returns></returns>
        public async Task<ProductGetCombinedInfoOutput> GetCombinedInfoAsync(string code, string soldCode)
        {
            var quotaCount = await quotaService.GetCountAsync(soldCode, code);
            var rebatePool = await rebatePoolService.GetAsync(soldCode, code);
            return await productRep.Value.Orm.Select<ProductEntity, ProductPriceEntity>()
                .InnerJoin((product, price) => product.Code == price.ProductCode)
                .Where(a => a.t1.Code == code && a.t2.SoldCode == soldCode)
                .ToOneAsync(a => new ProductGetCombinedInfoOutput
                {
                    QuotaCount = quotaCount,
                    RebatePoolAmount = rebatePool.Amount
                });
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<ProductDto>> GetPageAsync(PageInput<ProductGetPageInput> input)
        {
            var list = await productRep.Value.Select
                .WhereIf(!string.IsNullOrEmpty(input.Filter.Key),
                    a => a.Code.Contains(input.Filter.Key) || a.Name.Contains(input.Filter.Key))
                .Count(out var total)
                .OrderByDescending(true, a => a.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync();

            var data = new PageOutput<ProductDto>()
            {
                List = Mapper.Map<List<ProductDto>>(list),
                Total = total
            };

            return data;
        }

        /// <summary>
        /// 价格查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<ProductPriceDto>> GetPricePageAsync(PageInput<ProductPriceGetPageInput> input)
        {
            var list = await productPriceRep.Value.Select
                .WhereIf(!string.IsNullOrEmpty(input.Filter.DealerKey),
                    a => a.Dealer.SoldCode.Contains(input.Filter.DealerKey) ||
                         a.Dealer.Name.Contains(input.Filter.DealerKey))
                .WhereIf(!string.IsNullOrEmpty(input.Filter.ProductKey),
                    a => a.Product.Code.Contains(input.Filter.ProductKey) ||
                         a.Product.Name.Contains(input.Filter.ProductKey))
                .Count(out var total)
                .OrderByDescending(true, a => a.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync(a => new ProductPriceDto
                {
                    ProductName = a.Product.Name,
                    DealerName = a.Dealer.Name
                });

            var data = new PageOutput<ProductPriceDto>()
            {
                List = list,
                Total = total
            };
            return data;
        }


        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProductDto> UpdateAsync(ProductUpdateInput input)
        {
            var product = await productRep.Value.GetAsync(input.Id);
            if (product == null)
            {
                throw ResultOutput.Exception("产品不存在");
            }

            product.BoxSize = input.BoxSize;
            product.CommonName = input.CommonName;
            await productRep.Value.UpdateAsync(product);
            return Mapper.Map<ProductDto>(productRep);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<string> ImportAsync(IFormFile formFile)
        {
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper.Map<ProductEntity>("产品名称", a => a.Name)
                .Map<ProductEntity>("产品代码", a => a.Code)
                .Map<ProductEntity>("规格", a => a.Specification)
                .Map<ProductEntity>("箱规", a => a.BoxSize)
                .Map<ProductEntity>("通用名", a => a.CommonName)
                .Map<ProductEntity>("单位", a => a.Unit);

            var products = mapper.Take<ProductEntity>().Select(i => i.Value).ToList();
            if (!products.Any())
            {
                throw ResultOutput.Exception("数据不能为空");
            }
            await productRep.Value.DeleteAsync(a => true);
            await productRep.Value.InsertAsync(products);
            return $"导入成功：{products.Count}条";
        }


        /// <summary>
        /// 导入价格
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<List<DealerImportOutput>> ImportPriceAsync(IFormFile formFile)
        {
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper.Map<ProductPriceEntity>("Customer", a => a.SoldCode)
                .Map<ProductPriceEntity>("Material", a => a.ProductCode)
                .Map<ProductPriceEntity>("Condition Amount", a => a.Amount);

            var productPrices = mapper.Take<ProductPriceEntity>().Select(i => i.Value).ToList();
            if (!productPrices.Any())
            {
                throw ResultOutput.Exception("数据不能为空");
            }
            var duplicateSoldCodes = productPrices
                .GroupBy(item => new { item.SoldCode, item.ProductCode })
                .Where(group => group.Count() > 1)
                .Select(group => $"[{group.Key.SoldCode},{group.Key.ProductCode}]").ToList();

            if (duplicateSoldCodes.Any())
            {
                throw ResultOutput.Exception($"经销商+产品代码重复：{string.Join(",", duplicateSoldCodes)}");
            }

            var output = new List<DealerImportOutput>();
            var rowId = 2;

            var dealers = await dealerService.GetListAsync();

            var products = await productRep.Value.Select.ToListAsync();

            foreach (var item in productPrices)
            {
                if (item.SoldCode.IsNullOrEmpty())
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"经销商代码不能为空！" });
                }
                else if (!dealers.Exists(a => a.SoldCode == item.SoldCode))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"经销商代码不存在！" });
                }
                if (item.ProductCode.IsNullOrEmpty())
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"产品代码不能为空！" });
                }
                else if (!products.Exists(a => a.Code == item.ProductCode))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"产品代码不存在！" });
                }

                if (item.Amount <= 0)
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"价格不能小等于0！" });
                }

            }

            if (output.Any())
            {
                return output;
            }

            await productRep.Value.ImportPriceAsync(productPrices);
            return output;
        }
    }
}
