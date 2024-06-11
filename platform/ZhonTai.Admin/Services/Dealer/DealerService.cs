using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npoi.Mapper;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.User;
using ZhonTai.Admin.Services.Dealer.Dto;
using ZhonTai.Admin.Services.User;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.Dealer
{
    /// <summary>
    /// 经销商服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class DealerService(Lazy<IDealerRepository> dealerRep, Lazy<IDealerAddressRepository> dealerAddressRep,
            Lazy<IDealerLicenseRepository> dealerLicenceRep, Lazy<IUserService> userService)
        : BaseService, IDealerService, IDynamicApi
    {


        private static readonly List<string> DealerStatusStrs = new() { "Active", "Inactive" };
        private static readonly List<string> DealerTypeStrs = new() { "一般经销商", "重点经销商" };

        /// <summary>
        /// 获取单个经销商详情(包括地址)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<DealerDetailDto> GetAsync(long id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException($"未输入经销商ID！");
            }
            var dealer = await dealerRep.Value.Select
                .IncludeMany(p => p.Addresses)
                .IncludeMany(p => p.Licenses)
                .Where(p => p.Id == id)
                .ToOneAsync(a => new DealerDetailDto
                {
                    Id = a.Id,
                    SoldCode = a.SoldCode,
                    Name = a.Name,
                    Region = a.Region,
                    Province = a.Province,
                    City = a.City,
                    BizUserName = a.BizUser.Name,
                    ExHubUserName = a.ExHubUser.Name,
                    RegionManagerUserName = a.RegionManager.Name,
                    Address = a.Address,
                    Receiver = a.Receiver,
                    ReceiverMobile = a.ReceiverMobile,
                    Addresses = a.Addresses.Select(p => new DealerAddressOutput
                    {
                        Address = p.Address,
                        ShipCode = p.ShipCode,
                        Receiver = p.Receiver,
                        ReceiverMobile = p.ReceiverMobile,
                    }).ToList(),
                    Licences = a.Licenses.Select(p => new DealerLicencesOutput
                    {
                        EffectiveDate = p.EffectiveDate,
                        ExpirationDate = p.ExpirationDate,
                        Name = p.Name,
                    }).ToList()
                });
            return dealer;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="key">模糊关键词</param>
        /// <returns></returns>
        public async Task<List<DealerGetListOutput>> GetListAsync(string key = "")
        {
            var soldCodes = new List<string>();
            if (!User.RoleNames.Contains(RoleNames.ExHub))
            {
                soldCodes.Add(User.SoldCode);
                soldCodes.AddRange(await dealerRep.Value.GetCurrentUserSoldCodesAsync());
            }
            var dealers = await Cache.GetOrSetAsync(CacheKeys.Dealers, () =>
             {
                 return dealerRep.Value.Select
                     .Where(a => a.Status == DealerStatus.Active)
                     .ToListAsync(a => new DealerGetListOutput());
             });
            if (soldCodes.Any())
            {
                dealers = dealers.Where(a => soldCodes.Contains(a.SoldCode)).ToList();
            }

            return !string.IsNullOrWhiteSpace(key) ? dealers.Where(a => a.SoldCode.Contains(key) || a.Name.Contains(key)).ToList() : dealers;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input">查询组合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<DealerGetListOutput>> GetPageAsync(PageInput<DealerGetListInPut> input)
        {
            var soldCodes = new List<string>();
            if (!User.RoleNames.Contains(RoleNames.ExHub))
            {
                soldCodes.Add(User.SoldCode);
                soldCodes.AddRange(await dealerRep.Value.GetCurrentUserSoldCodesAsync());
            }

            var list = await dealerRep.Value.Select
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Key),
                    p => p.Name.Contains(input.Filter.Key) || p.SoldCode.Contains(input.Filter.Key))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Region), p => p.Region == input.Filter.Region)
                .WhereIf(soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
                .Count(out var total)
                .OrderByDescending(true, a => a.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync(a => new DealerGetListOutput
                {
                    DealerStatus = a.Status,
                    BizUserRealName = a.BizUser.Name,
                    ExHubUserRealName = a.ExHubUser.Name,
                    RegionManagerRealName = a.RegionManager.Name
                });

            var data = new PageOutput<DealerGetListOutput>()
            {
                List = list,
                Total = total
            };

            return data;
        }

        /// <summary>
        /// 查询地址列表
        /// </summary>
        /// <param name="soldCode">经销商代码</param>
        /// <returns></returns>
        public async Task<List<DealerAddressListOutput>> GetAddressListAsync(string soldCode = "")
        {
            var dealerAddresses = await Cache.GetOrSetAsync(CacheKeys.DealerAddresses, () => dealerAddressRep.Value.Select
                .ToListAsync<DealerAddressListOutput>());

            return !string.IsNullOrWhiteSpace(soldCode) ? dealerAddresses.Where(a => a.SoldCode == soldCode).ToList() : dealerAddresses;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<DealerImportOutput>> ImportAsync(IFormFile formFile)
        {
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper.Map<DealerImportDto>("商业名称", a => a.Name)
                .Map<DealerImportDto>("Sold to", a => a.SoldCode)
                .Map<DealerImportDto>("区域", a => a.Region)
                .Map<DealerImportDto>("省份", a => a.Province)
                .Map<DealerImportDto>("城市", a => a.City)
                .Map<DealerImportDto>("大区经理", a => a.RegionManagerEmail)
                .Map<DealerImportDto>("地区商务", a => a.BizUserEmail)
                .Map<DealerImportDto>("EX HUB负责人", a => a.ExHubUserEmail)
                .Map<DealerImportDto>("状态", a => a.StatusStr)
                .Map<DealerImportDto>("经销商类型", a => a.TypeStr)
                .Map<DealerImportDto>("联系地址", a => a.Address)
                .Map<DealerImportDto>("联系人", a => a.Receiver)
                .Map<DealerImportDto>("联系电话", a => a.ReceiverMobile);

            var dealers = mapper.Take<DealerImportDto>().Select(i => i.Value).ToList();

            if (!dealers.Any())
            {
                throw ResultOutput.Exception("数据不能为空");
            }

            var duplicateSoldCodes = dealers
                .GroupBy(item => item.SoldCode)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).ToList();

            if (duplicateSoldCodes.Any())
            {
                throw ResultOutput.Exception($"存在重复的SoldCode：{string.Join(",", duplicateSoldCodes)}");
            }

            // 循环校验
            var output = new List<DealerImportOutput>();
            var rowId = 2;
            var users = await userService.Value.GetAllAsync();
            foreach (var dealer in dealers)
            {
                if (string.IsNullOrWhiteSpace(dealer.SoldCode))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"Sold to不可为空！" });
                }
                if (string.IsNullOrWhiteSpace(dealer.Region))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"区域不可为空！" });
                }

                if (string.IsNullOrWhiteSpace(dealer.Province))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"省份不可为空！" });
                }

                if (string.IsNullOrWhiteSpace(dealer.City))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"城市不可为空！" });
                }

                if (!dealer.RegionManagerEmail.IsNullOrEmpty() && !users.Exists(a => a.Email == dealer.RegionManagerEmail))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"大区经理不存在！" });
                }
                else
                {
                    dealer.RegionManagerUserId = users.First(a => a.Email == dealer.RegionManagerEmail).Id;
                }

                if (!dealer.BizUserEmail.IsNullOrEmpty() && !users.Exists(a => a.Email == dealer.BizUserEmail))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"地区商务不存在！" });
                }
                else
                {
                    dealer.BizUserId = users.First(a => a.Email == dealer.BizUserEmail).Id;
                }

                if (!dealer.ExHubUserEmail.IsNullOrEmpty() && !users.Exists(a => a.Email == dealer.ExHubUserEmail))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"EX HUB负责人不存在！" });
                }
                else
                {
                    dealer.ExHubUserId = users.First(a => a.Email == dealer.ExHubUserEmail).Id;
                }

                if (string.IsNullOrWhiteSpace(dealer.Name))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"商业名称不可为空！" });
                }

                if (string.IsNullOrWhiteSpace(dealer.StatusStr))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"状态不可为空！" });
                }
                else if (!DealerStatusStrs.Contains(dealer.StatusStr))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"状态只可为Active或Inactive！" });
                }

                if (string.IsNullOrWhiteSpace(dealer.TypeStr))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"经销商类型不可为空！" });
                }
                else if (!DealerTypeStrs.Contains(dealer.TypeStr))
                {
                    output.Add(new DealerImportOutput { RowId = rowId, ErrorMsg = $"经销商类型只可为一般经销商或重点经销商！" });
                }
                rowId++;
            }

            // 有错误则直接返回
            if (output.Any())
            {
                return output;
            }

            var dealerEntities = Mapper.Map<List<DealerEntity>>(dealers);

            dealerRep.Value.DbContextOptions.EnableCascadeSave = true;

            await dealerRep.Value.DeleteCascadeByDatabaseAsync(a => true);
            await dealerRep.Value.InsertAsync(dealerEntities);
            await Cache.DelAsync(CacheKeys.Dealers);
            return output;
        }

        /// <summary>
        /// 导入地址
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<string> ImportAddressAsync(IFormFile formFile)
        {
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper.Map<DealerAddressEntity>("SoldCode", a => a.SoldCode)
                .Map<DealerAddressEntity>("ShipCode", a => a.ShipCode)
                .Map<DealerAddressEntity>("地址", a => a.Address);

            var dealerAddresses = mapper.Take<DealerAddressEntity>("经销商地址").Select(i => i.Value);
            var dealerAddressEntities = dealerAddresses as DealerAddressEntity[] ?? dealerAddresses.ToArray();
            await dealerAddressRep.Value.DeleteAsync(a => true);
            await dealerAddressRep.Value.InsertAsync(dealerAddressEntities);
            await Cache.DelAsync(CacheKeys.DealerAddresses);
            return $"导入成功：{dealerAddressEntities.Length}条";
        }

        [NonAction]
        public Task<List<string>> GetCurrentUserSoldCodesAsync()
        {
            return dealerRep.Value.GetCurrentUserSoldCodesAsync();
        }

        /// <summary>
        /// 导入证照
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<List<LicenceImportOutput>> ImportLicenceAsync(IFormFile formFile)
        {
            var mapper = new Mapper(formFile.OpenReadStream())
            {
                TrimSpaces = TrimSpacesType.Both,
                SkipBlankRows = true
            };
            mapper.Map<DealerLicenseEntity>("客户编码", a => a.SoldCode)
                .Map<DealerLicenseEntity>("证照名称", a => a.Name)
                .Map<DealerLicenseEntity>("证照生效日期", a => a.EffectiveDate)
                .Map<DealerLicenseEntity>("证照失效日期", a => a.ExpirationDate);

            var licenses = mapper.Take<DealerLicenseEntity>().Select(i => i.Value).ToList();
            if (!licenses.Any())
            {
                throw ResultOutput.Exception("数据不能为空");
            }

            // 循环校验
            var output = new List<LicenceImportOutput>();
            var rowId = 2;

            // 获取全部经销商和产品
            var dealers = await GetListAsync();
            foreach (var license in licenses)
            {
                if (string.IsNullOrWhiteSpace(license.SoldCode))
                {
                    output.Add(new LicenceImportOutput { RowId = rowId, ErrorMsg = $"客户编码不可为空！" });
                }
                else if (dealers.All(a => license.SoldCode != a.SoldCode))
                {
                    output.Add(new LicenceImportOutput { RowId = rowId, ErrorMsg = $"客户编码在系统中不存在！" });
                }

                if (string.IsNullOrWhiteSpace(license.Name))
                {
                    output.Add(new LicenceImportOutput { RowId = rowId, ErrorMsg = $"证照名称不可为空！" });
                }

                if (license.EffectiveDate == null)
                {
                    output.Add(new LicenceImportOutput { RowId = rowId, ErrorMsg = $"证照生效日期不可为空！" });
                }

                if (license.ExpirationDate == null)
                {
                    output.Add(new LicenceImportOutput { RowId = rowId, ErrorMsg = $"证照失效日期不可为空！" });
                }

                if (license.EffectiveDate.HasValue && license.ExpirationDate.HasValue && license.ExpirationDate.Value <= license.EffectiveDate.Value)
                {
                    output.Add(new LicenceImportOutput { RowId = rowId, ErrorMsg = $"证照失效日期应大于证照生效日期" });
                }
                rowId++;
            }

            // 有错误则直接返回
            if (output.Any())
            {
                return output;
            }
            dealerLicenceRep.Value.DbContextOptions.EnableCascadeSave = true;

            await dealerLicenceRep.Value.DeleteCascadeByDatabaseAsync(a => true);
            await dealerLicenceRep.Value.InsertAsync(licenses);
            return output;
        }
    }
}
