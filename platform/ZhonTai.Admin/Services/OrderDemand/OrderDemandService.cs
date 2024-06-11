using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.DynamicApi.Attributes;
using ZhonTai.DynamicApi;
using ZhonTai.Admin.Domain.OrderDemand;
using ZhonTai.Admin.Services.OrderDemand.Dto;
using ZhonTai.Admin.Services.SerialNo;
using ZhonTai.Admin.Domain.SerialNo;
using ZhonTai.Admin.Services.Order;
using ZhonTai.Admin.Services.Product;
using System.IO;
using Npoi.Mapper;
using ZhonTai.Admin.Services.Quota;

namespace ZhonTai.Admin.Services.OrderDemand
{
    /// <summary>
    /// 订单需求服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class OrderDemandService : BaseService, IOrderDemandService, IDynamicApi
    {
        private readonly Lazy<IOrderDemandRepository> _orderDemandRep;
        private readonly Lazy<IDealerRepository> _dealerRep;
        private readonly ISerialNoService _serialNoService;
        private readonly IProductService _productService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IQuotaService> _quotaService;

        private readonly Dictionary<OrderDemandStatus, List<OrderDemandStatus>> _rules = new()
        {
            {
                OrderDemandStatus.Completed,new List<OrderDemandStatus>{ OrderDemandStatus.Submitted, OrderDemandStatus.Edited }
            },
            {
                OrderDemandStatus.Returned,new List<OrderDemandStatus>{ OrderDemandStatus.Submitted, OrderDemandStatus.Edited }
            }
        };


        public OrderDemandService(
            Lazy<IOrderDemandRepository> orderDemandRep,
            IProductService productService,
            ISerialNoService serialNoService,
            Lazy<IDealerRepository> dealerRepository,
            Lazy<IOrderService> orderService,
            Lazy<IQuotaService> quotaService)
        {
            _orderDemandRep = orderDemandRep;
            _productService = productService;
            _serialNoService = serialNoService;
            _dealerRep = dealerRepository;
            _orderService = orderService;
            _quotaService = quotaService;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input">查询条件组合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<OrderDemandGetPageOutput>> GetPageAsync(PageInput<OrderDemandGetPageInput> input)
        {
            var beginDate = input.Filter.CreatedTimeFrom;
            var endDate = input.Filter.CreatedTimeTo.HasValue ? input.Filter.CreatedTimeTo.Value.AddDays(1) : input.Filter.CreatedTimeTo;
            var soldCodes = new List<string>();
            if (!User.RoleNames.Contains(RoleNames.ExHub))
            {
                soldCodes.Add(User.SoldCode);
                soldCodes.AddRange(await _dealerRep.Value.GetCurrentUserSoldCodesAsync());
            }
            var list = await _orderDemandRep.Value.Select
                .WhereIf(soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Name),
                    p => p.PrCode.Contains(input.Filter.Name) || p.DealerName.Contains(input.Filter.Name))
                .WhereIf(input.Filter.Status != null, p => p.Status == input.Filter.Status)
                .WhereIf(input.Filter.IsUrgent != null, p => p.IsUrgent == input.Filter.IsUrgent)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.CreatedUserName),
                    p => p.CreatedUserName == input.Filter.CreatedUserName)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Region), p => p.Region == input.Filter.Region)
                .WhereIf(input.Filter.CreatedTimeFrom != null, p => p.CreatedTime >= beginDate)
                .WhereIf(input.Filter.CreatedTimeTo != null, p => p.CreatedTime <= endDate)
                .WhereIf(input.Filter.ShowDifference, a => a.Orders.Sum(o => o.Count) != a.Items.Sum(i => i.NeedCount))
                .Count(out var total)
                .OrderByDescending(true, p => p.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync(p => new OrderDemandGetPageOutput
                {
                    Id = p.Id,
                    SoldCode = p.SoldCode,
                    ShipCode = p.ShipCode,
                    IsUrgent = p.IsUrgent,
                    Remark = p.Remark,
                    PrCode = p.PrCode,
                    DealerName = p.DealerName,
                    Status = p.Status,
                    Region = p.Region,
                    CreatedTime = p.CreatedTime,
                    CreatedUserName = p.CreatedUserName,
                    ModifiedTime = p.ModifiedTime,
                    ModifiedUserName = p.ModifiedUserName,
                    CreatedUserRealName = p.CreatedUserRealName,
                    ModifiedUserRealName = p.ModifiedUserRealName,
                    SoTotalAmount = p.Orders.Sum(o => o.Count),
                    PrTotalAmount = p.Items.Sum(i => i.NeedCount)
                });

            var data = new PageOutput<OrderDemandGetPageOutput>
            {
                List = list,
                Total = total
            };

            return data;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [HttpPost]
        public async Task<long> SubmitAsync(OrderDemandInsertOrUpdateInput request)
        {
            await SubmitCheckAsync(request);

            // 查询是否已存在该ID
            var orderDemand = await _orderDemandRep.Value.Select
                .Where(p => p.Id == request.Id)
                .ToOneAsync();

            // 默认为已提交，如若有，则为已修改
            OrderDemandStatus status = OrderDemandStatus.Submitted;

            if (orderDemand is { Status: OrderDemandStatus.Returned })
            {
                status = OrderDemandStatus.Edited;
            }

            return await InsertOrUpdateAsync(request, status);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<long> SaveAsync(OrderDemandInsertOrUpdateInput request)
        {
            return InsertOrUpdateAsync(request, OrderDemandStatus.Saved); ;
        }

        /// <summary>
        /// 提交检查
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task SubmitCheckAsync(OrderDemandInsertOrUpdateInput input)
        {
            foreach (var item in input.Items)
            {
                var product = await _productService.GetCombinedInfoAsync(item.ProductCode, input.SoldCode);
                // 折扣后金额不可小于0
                var afterDiscountAmount = item.NeedCount * product.Amount - item.RebateAmount;
                if (afterDiscountAmount < 0)
                {
                    throw ResultOutput.Exception($"{item.ProductCode} ：折扣后金额不可小于0！");
                }
                // 剩余配额不可超支
                var remainQuota = product.QuotaCount - item.NeedCount;
                if (remainQuota < 0)
                {
                    throw ResultOutput.Exception($"{item.ProductCode} ：下单量超出剩余配额！");
                }
            }
        }

        /// <summary>
        /// 处理是新增还是修改订单需求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [NonAction]
        private async Task<long> InsertOrUpdateAsync(OrderDemandInsertOrUpdateInput request, OrderDemandStatus status)
        {
            // 获取经销商和地址
            var dealer = await _dealerRep.Value.GetWithAddressAsync(request.SoldCode);
            var dealerAddress = dealer.Addresses?.FirstOrDefault(a => a.ShipCode == request.ShipCode);

            if (dealer == null || dealerAddress == null)
            {
                throw ResultOutput.Exception("经销商信息不存在");
            }

            var orderDemandEntity = await _orderDemandRep.Value.Select.Where(p => p.Id == request.Id).ToOneAsync() ?? new OrderDemandEntity();
            orderDemandEntity.DealerName = dealer.Name;
            orderDemandEntity.DealerAddress = dealerAddress.Address;
            orderDemandEntity.SoldCode = request.SoldCode;
            orderDemandEntity.ShipCode = request.ShipCode;
            orderDemandEntity.IsUrgent = request.IsUrgent;
            orderDemandEntity.Status = status == OrderDemandStatus.Saved ? orderDemandEntity.Status : status;
            orderDemandEntity.Remark = request.Remark;
            orderDemandEntity.Region = dealer.Region;


            foreach (var item in request.Items)
            {
                var productDetail = await _productService.GetCombinedInfoAsync(item.ProductCode, request.SoldCode);

                if (productDetail == null)
                {
                    throw ResultOutput.Exception($"该经销商下无此产品 : {item.ProductCode} ");
                }

                if (orderDemandEntity.Status == OrderDemandStatus.Submitted)
                {
                    var count = await _quotaService.Value.GetCountAsync(request.SoldCode, item.ProductCode);
                    if (item.NeedCount > count)
                    {
                        throw ResultOutput.Exception($"该产品需求量超过剩余配额 : {item.ProductCode} ");
                    }
                }

                var orderDemandItemEntity = new OrderDemandItemEntity
                {
                    OrderDemandId = orderDemandEntity.Id,
                    ProductCode = item.ProductCode,
                    NeedCount = item.NeedCount,
                    RebateAmount = item.RebateAmount,
                    AcceptPartialBox = item.AcceptPartialBox,
                    IsUseDiscount = item.IsUseDiscount,
                    ProductName = productDetail.Name,
                    Specification = productDetail.Specification,
                    Unit = productDetail.Unit,
                    Amount = productDetail.Amount,
                    BoxSize = productDetail.BoxSize,
                    RemainingQuota = productDetail.QuotaCount
                };
                orderDemandEntity.Items.Add(orderDemandItemEntity);

            }

            if (orderDemandEntity.Status == OrderDemandStatus.Submitted)
            {
                orderDemandEntity.FirstCommitTime ??= DateTime.Now;
                orderDemandEntity.PrCode = await _serialNoService.GetSerialNoAsync(SerialNoType.OrderDemand);
            }

            //orderDemandEntity.Items.ForEach(p => p.OrderDemandId = orderDemandEntity.Id);
            _orderDemandRep.Value.DbContextOptions.EnableCascadeSave = true;
            await _orderDemandRep.Value.DeleteItemAsync(orderDemandEntity.Id);
            await _orderDemandRep.Value.InsertOrUpdateAsync(orderDemandEntity);
            return orderDemandEntity.Id;

        }

        /// <summary>
        /// 查询聚合（For编辑）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderDemandGetCombinedOutput> GetCombinedAsync(long id)
        {
            // 查询订单需求
            var orderDemand = await _orderDemandRep.Value.Select
                .IncludeMany(p => p.Items)
                .Where(p => p.Id == id)
                .ToOneAsync();

            if (orderDemand == null)
            {
                throw ResultOutput.Exception("记录不存在");
            }

            var dto = Mapper.Map<OrderDemandGetCombinedOutput>(orderDemand);
            foreach (var item in dto.Items)
            {
                var product = await _productService.GetCombinedInfoAsync(item.ProductCode, dto.SoldCode);
                Mapper.Map(product, item);
            }
            return dto;

        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderDemandGetOutput> GetAsync(long id)
        {
            // 查询订单需求
            var orderDemand = await _orderDemandRep.Value.Select
                .IncludeMany(p => p.Items)
                .Where(p => p.Id == id)
                .ToOneAsync();

            if (orderDemand == null)
            {
                throw ResultOutput.Exception("记录不存在");
            }

            var dto = Mapper.Map<OrderDemandGetOutput>(orderDemand);
            if (!string.IsNullOrEmpty(dto.PrCode))
            {
                var orders = await _orderService.Value.GetListAsync(dto.PrCode);
                var orderDemandOrders = new List<OrderDemandOrderDto>();
                foreach (var order in orders)
                {
                    foreach (var item in order.Items)
                    {
                        var orderDto = new OrderDemandOrderDto
                        {
                            SoCode = order.SoCode,
                            ReferenceCode = order.ReferenceCode,
                            ProductCode = item.ProductCode,
                            Count = item.Count,
                            TotalAmount = item.TotalAmount
                        };
                        orderDemandOrders.Add(orderDto);
                    }
                }

                foreach (var item in dto.Items)
                {
                    item.Orders = orderDemandOrders.Where(a => a.ProductCode == item.ProductCode).ToList();
                }
            }
            return dto;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task CompleteAsync(long id)
        {
            var orderDemand = await _orderDemandRep.Value.Select
                .IncludeMany(p => p.Items)
                .Where(p => p.Id == id)
                .ToOneAsync();
            if (orderDemand == null)
            {
                throw ResultOutput.Exception("订单需求不存在");
            }
            if (!_rules[OrderDemandStatus.Completed].Contains(orderDemand.Status))
            {
                throw ResultOutput.Exception($"当前状态不允许完成");
            }
            orderDemand.Status = OrderDemandStatus.Completed;
            await _orderDemandRep.Value.UpdateAsync(orderDemand);
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task ReturnAsync(long id)
        {
            var orderDemand = await _orderDemandRep.Value.Select
                .IncludeMany(p => p.Items)
                .Where(p => p.Id == id)
                .ToOneAsync();
            if (orderDemand == null)
            {
                throw ResultOutput.Exception("订单需求不存在");
            }
            if (!_rules[OrderDemandStatus.Returned].Contains(orderDemand.Status))
            {
                throw ResultOutput.Exception($"当前状态不允许撤回");
            }
            orderDemand.Status = OrderDemandStatus.Returned;
            await _orderDemandRep.Value.UpdateAsync(orderDemand);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(long id)
        {
            // 获取需求订单
            var orderDemand = await _orderDemandRep.Value.Select
                .Where(p => p.Id == id)
                .ToOneAsync();
            if (orderDemand == null)
            {
                throw new ArgumentOutOfRangeException($"ID {id} is not found, delete error! ");
            }

            try
            {
                orderDemand.Status = OrderDemandStatus.Deleted;
                await _orderDemandRep.Value.UpdateAsync(orderDemand);
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException($"Delete error! Message: {ex.Message}");
            }

            return $"Delete success! ";
        }


        /// <summary>
        /// 导出订单需求
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileContentResult> ExportAsync(OrderDemandGetPageInput input)
        {
            var beginDate = input.CreatedTimeFrom;
            var endDate = input.CreatedTimeTo.HasValue ? input.CreatedTimeTo.Value.AddDays(1) : input.CreatedTimeTo;
            if (beginDate == null || endDate == null)
            {
                throw ResultOutput.Exception("请选择完整的时间范围");
            }

            var soldCodes = new List<string>();
            if (!User.RoleNames.Contains(RoleNames.ExHub))
            {
                soldCodes.Add(User.SoldCode);
                soldCodes.AddRange(await _dealerRep.Value.GetCurrentUserSoldCodesAsync());
            }
            var orderDemands = await _orderDemandRep.Value.Select
              //.Where(a => soldCodes.Contains(a.SoldCode))
              // 经销商名称/PR Code
              .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                  p => p.PrCode.Contains(input.Name) || p.DealerName.Contains(input.Name))
              .WhereIf(input.Status != null, p => p.Status == input.Status)
              .WhereIf(input.IsUrgent != null, p => p.IsUrgent == input.IsUrgent)
              .WhereIf(!string.IsNullOrWhiteSpace(input.CreatedUserName),
                  p => p.CreatedUserName == input.CreatedUserName)
              .WhereIf(soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
              .WhereIf(!string.IsNullOrWhiteSpace(input.Region), p => p.Region == input.Region)
              .WhereIf(input.CreatedTimeFrom != null, p => p.CreatedTime >= beginDate)
              .WhereIf(input.CreatedTimeTo != null, p => p.CreatedTime <= endDate)
              .WhereIf(input.ShowDifference, a => a.Orders.Sum(o => o.Count) != a.Items.Sum(i => i.NeedCount))
              .IncludeMany(a => a.Items)
              .IncludeMany(a => a.Orders)
              .OrderByDescending(true, p => p.Id)
              .ToListAsync();

            var list = new List<OrderDemandExportDto>();

            foreach (var orderDemand in orderDemands)
            {
                foreach (var item in orderDemand.Items)
                {
                    var dto = new OrderDemandExportDto
                    {
                        PrCode = orderDemand.PrCode,
                        DealerName = orderDemand.DealerName,
                        Status = orderDemand.Status.ToDescriptionOrString(),
                        PrCount = orderDemand.Items.Sum(a => a.NeedCount),
                        SoCount = orderDemand.Orders.Sum(a => a.Count),
                        ModifiedTime = orderDemand.ModifiedTime ?? orderDemand.CreatedTime,
                        CreatedUserRealName = orderDemand.CreatedUserRealName,
                        Region = orderDemand.Region,
                        SoldCode = orderDemand.SoldCode,
                        ShipCode = orderDemand.ShipCode,
                        DealerAddress = orderDemand.DealerAddress,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Specification = item.Specification,
                        Amount = item.Amount,
                        ProductCount = item.NeedCount,
                        BoxCount = Math.Round((decimal)item.NeedCount / item.BoxSize, 1),
                        AcceptPartialBox = item.AcceptPartialBox ? "是" : "否",
                        TotalAmount = item.NeedCount * item.Amount,
                        IsUseDiscount = item.IsUseDiscount ? "是" : "否",
                        RebateAmount = item.RebateAmount,
                        AfterDiscountAmount = item.NeedCount * item.Amount - item.RebateAmount,
                        IsUrgent = orderDemand.IsUrgent ? "是" : "否",
                        Remark = orderDemand.Remark
                    };
                    list.Add(dto);
                }
            }

            var mapper = new Mapper();
            MemoryStream stream = new MemoryStream();

            mapper.Save(stream, list, "Sheet1", false);

            return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"订单需求导出_{DateTime.Now:yyyyMMddHHmm}.xlsx"
            };
        }
    }
}
