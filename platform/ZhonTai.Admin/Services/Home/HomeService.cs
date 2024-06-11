using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicExtensions;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Order;
using ZhonTai.Admin.Domain.OrderDemand;
using ZhonTai.Admin.Services.Auth;
using ZhonTai.Admin.Services.Home.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.Home
{
    /// <summary>
    /// 首页服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class HomeService : BaseService, IDynamicApi
    {
        private readonly IDealerRepository _dealerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDemandRepository _orderDemandRepository;
        private readonly IAuthService _authService;

        public HomeService(IDealerRepository dealerRepository, IOrderRepository orderRepository, IOrderDemandRepository orderDemandRepository, IAuthService authService)
        {
            _dealerRepository = dealerRepository;
            _orderRepository = orderRepository;
            _orderDemandRepository = orderDemandRepository;
            _authService = authService;
        }

        /// <summary>
        /// 获取汇总数据
        /// </summary>
        /// <returns></returns>
        public async Task<HomeGetSummaryOutput> GetSummaryAsync()
        {
            var summary = new HomeGetSummaryOutput();
            var soldCodes = new List<string>();
            if (!User.RoleNames.Contains(RoleNames.ExHub))
            {
                soldCodes.Add(User.SoldCode);
                soldCodes.AddRange(await _dealerRepository.GetCurrentUserSoldCodesAsync());
            }

            var permissions = await _authService.GetUserPermissionsAsync();
            var inside = permissions.Permissions.Contains("api:admin:home:inside");

            if (inside)
            {
                summary.DealerCount = soldCodes.Count(a => !a.IsNullOrEmpty());

                summary.OrderDemandCount = (int)await _orderDemandRepository.Select
                    .Where(a =>
                        a.FirstCommitTime.Value.Year == DateTime.Now.Year &&
                        a.FirstCommitTime.Value.Month == DateTime.Now.Month)
                    .Where(a => a.Status == OrderDemandStatus.Submitted || a.Status == OrderDemandStatus.Edited)
                    .WhereIf(!soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
                    .CountAsync();
            }

            summary.OrderCount = (int)await _orderRepository.Select.Where(a =>
                     a.SapCreatedTime.Value.Year == DateTime.Now.Year &&
                     a.SapCreatedTime.Value.Month == DateTime.Now.Month &&
                     a.Status != OrderStatus.Deleted)
                .WhereIf(!soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
                .CountAsync();

            var items = await _orderRepository.Select
                .Where(a =>
                    a.SapCreatedTime.Value.Year == DateTime.Now.Year &&
                    a.SapCreatedTime.Value.Month == DateTime.Now.Month &&
                    a.Status != OrderStatus.Deleted)
                .WhereIf(!soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
                .ToListAsync(a=>a.Items.Sum(b=>b.Count));

            summary.OrderProductCount = items.Sum();
            return summary;
        }
    }
}
