using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NuoNuoSdk;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Order;
using ZhonTai.Admin.Services.Dealer;
using ZhonTai.Admin.Services.Order.Dto;
using ZhonTai.Admin.Services.Order.NuoNuo;
using ZhonTai.Admin.Services.Product;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;
using ZhonTai.Admin.Domain.Contract;
using ZhonTai.Admin.Domain.Express;
using LogicExtensions;
using ZhonTai.Common.Extensions;
using NPOI.SS.Formula.Functions;

namespace ZhonTai.Admin.Services.Order;

/// <summary>
/// 订单服务
/// </summary>
[DynamicApi(Area = AdminConsts.AreaName)]
public class OrderService(Lazy<IOrderRepository> orderRep,
        Lazy<IDealerService> dealerService,
        Lazy<IProductService> productService,
        Lazy<INuoNuoSdk> nuoNuoSdk,
        Lazy<IContractRepository> contractRep,
        Lazy<IExpressRepository> expressRep,
        Lazy<IInvoiceRepository> invoiceRep)
    : BaseService, IOrderService, IDynamicApi
{
    private readonly IList<string> _allowExtension = new List<string>() { ".xlsx", ".xls" };


    /// <summary>
    /// 查询订单分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PageOutput<OrderGetPageOutput>> GetPageAsync(PageInput<OrderGetPageInput> input)
    {
        var soldCodes = new List<string>();
        if (!User.RoleNames.Contains(RoleNames.ExHub))
        {
            soldCodes.Add(User.SoldCode);
            soldCodes.AddRange(await dealerService.Value.GetCurrentUserSoldCodesAsync());
        }
        var orders = await orderRep.Value.Select
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Key), a => a.DealerName.Contains(input.Filter.Key) ||
            a.SoCode.Contains(input.Filter.Key) || a.PrCode.Contains(input.Filter.Key))
            .WhereIf(soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
            .WhereIf(input.Filter.Status != null, a => a.Status == input.Filter.Status)
            .WhereIf(input.Filter.SapCreatedTimeFrom != null, a => a.SapCreatedTime.Value.Date >= input.Filter.SapCreatedTimeFrom.Value.Date)
            .WhereIf(input.Filter.SapCreatedTimeTo != null, a => a.SapCreatedTime.Value.Date <= input.Filter.SapCreatedTimeFrom.Value.AddDays(1))
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.Region), a => a.Region == input.Filter.Region)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter.CreatedUserName), a => a.CreatedUserName.Contains(input.Filter.CreatedUserName))
            .Count(out var total)
            .Page(input.CurrentPage, input.PageSize)
            .OrderByDescending(a => a.Id)
            .ToListAsync(a => new OrderGetPageOutput
            {
                PrCode = a.PrCode,
                SoCode = a.SoCode,
                DealerName = a.DealerName,
                Count = a.Count,
                Status = a.Status,
                CreatedTime = a.CreatedTime,
                Region = a.Region,
                SapCreatedBy = a.SapCreatedBy,
                SapCreatedTime = a.SapCreatedTime,
                CreatedUserName = a.CreatedUserName,
                ModifiedTime = a.ModifiedTime,
                ModifiedUserRealName = a.ModifiedUserRealName,
                ModifiedUserName = a.ModifiedUserName,
                CreatedUserRealName = a.CreatedUserRealName,
                ContractFileId = a.Contract.FileId,
                ContractId = a.Contract.Id,
            });

        var result = new PageOutput<OrderGetPageOutput>()
        {
            Total = total,
            List = orders
        };
        return result;
    }

    /// <summary>
    /// 查询列表
    /// </summary>
    /// <param name="prCode"></param>
    /// <returns></returns>
    public async Task<List<OrderGetOutput>> GetListAsync(string prCode)
    {
        var orders = await orderRep.Value.Select
            .Where(a => a.PrCode == prCode)
            .IncludeMany(a => a.Items)
            .ToListAsync();
        return Mapper.Map<List<OrderGetOutput>>(orders);
    }

    /// <summary>
    /// 查询单个订单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OrderGetOutput> GetAsync(long id)
    {
        var order = await orderRep.Value.Select.Where(p => p.Id == id)
            .IncludeMany(a => a.Invoices)
            .IncludeMany(p => p.Items).ToOneAsync();
        if (order == null)
        {
            throw ResultOutput.Exception("订单不存在");
        }
        var res = Mapper.Map<OrderGetOutput>(order);
        //合同信息
        var contract = await contractRep.Value.Select.Where(p => p.SoCode == order.SoCode).Include(p => p.File).ToOneAsync();

        if (contract != null)
        {
            res.Contract = new OrderContractDto()
            {
                CreatedTime = contract.CreatedTime,
                FileId = contract.FileId,
                FileName = contract.File.FileName + contract.File.Extension,
                ContractId = contract.Id
            };
        }
        //物流信息
        var express = await expressRep.Value.Select.Where(p => p.SoCode == order.SoCode).IncludeMany(p => p.ExpressBatch).ToOneAsync();
        if (express != null)
        {
            res.Express = Mapper.Map<OrderExpressDto>(express);
            res.ExpressBatches = Mapper.Map<List<OrderExpressBatchDto>>(express.ExpressBatch);
        }

        //发票信息

        if (order.Invoices != null)
        {

            foreach (var invoice in order.Invoices)
            {
                var invoices = await GetInvoices(new List<string> { invoice.Billing });
                var invoiceDto = new OrderInvoiceDto
                {
                    GtsNo = invoice.Billing,
                    InvoiceCode = invoice.No,
                    InvoiceTime = invoice.Date
                };
                if (invoices.Success)
                {
                    var nuoInvoice = invoices.Result.FirstOrDefault(a => a.Status == "2");
                    invoiceDto.OfdUrl = nuoInvoice?.OfdUrl;
                    invoiceDto.PdfUrl = nuoInvoice?.PdfUrl;
                    invoiceDto.XmlUrl = nuoInvoice?.XmlUrl;
                }
                res.Invoices.Add(invoiceDto);
            }
        }

        return res;
    }

    /// <summary>
    /// 导入
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    public async Task<List<ImportOutput>> ImportAsync(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
        {
            throw ResultOutput.Exception("请上传Excel文件");
        }

        var extension = Path.GetExtension(formFile.FileName);
        if (!_allowExtension.Contains(extension))
            throw ResultOutput.Exception("不支持的文件格式");



        // 读取Excel字段
        var mapper = new Mapper(formFile.OpenReadStream())
        {
            TrimSpaces = TrimSpacesType.Both,
            SkipBlankRows = true
        };

        mapper.Map<OrderImportDto>("Sales Doc.", a => a.SoCode)
            .Map<OrderImportDto>("Cust. Ref. (Header)", a => a.ReferenceCode)
            .Map<OrderImportDto>("Sold-To", a => a.SoldCode)
            .Map<OrderImportDto>("Sold-To Party Name", a => a.DealerName)
            .Map<OrderImportDto>("Material", a => a.ProductCode)
            .Map<OrderImportDto>("Material Description", a => a.ProductName)
            .Map<OrderImportDto>("OrdQty (I)", a => a.Count)
            .Map<OrderImportDto>("NV (Item)", a => a.TotalAmount)
            .Map<OrderImportDto>("Del. Block", a => a.DeliveryBlock)
            .Map<OrderImportDto>("Created on", a => a.SapCreatedTime)
            .Map<OrderImportDto>("Created by", a => a.SapCreatedBy)
            .Map<OrderImportDto>("Batch", a => a.Batch)
            .Map<OrderImportDto>("SalesDocTy", a => a.OrderType)
            .Map<OrderImportDto>("Rej. Reasn", a => a.CancelReason)
            .Map<OrderImportDto>("Ship-to pa", a => a.ShipCode);

        var orders = mapper.Take<OrderImportDto>().Select(i => i.Value).Where(a => a.OrderType == "ZOR").ToList();

        if (!orders.Any())
        {
            throw ResultOutput.Exception("数据不能为空");
        }

        // 获取全部经销商和产品
        var dealers = await dealerService.Value.GetListAsync();
        var addresses = await dealerService.Value.GetAddressListAsync();
        var products = await productService.Value.GetListAsync();


        // 循环校验
        var output = new List<ImportOutput>();
        var rowId = 2;
        foreach (var order in orders)
        {
            if (order.ReferenceCode == null)
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"ReferenceCode不可为空！" });
            }

            if (order.SoCode == null)
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"SoCode不可为空！" });
            }

            if (order.SoldCode == null)
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"经销商代码不可为空！" });
            }

            if (dealers.All(p => p.SoldCode != order.SoldCode))
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"该经销商不存在！" });
            }

            if (order.ProductCode == null)
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"产品代码不可为空！" });
            }

            if (products.All(p => p.Code != order.ProductCode))
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"该产品不存在！" });
            }
            if (string.IsNullOrWhiteSpace(order.ShipCode))
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"ShipTo不可为空！" });
            }
            if (order.Count <= 0)
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"下单量不能为空或小于等于0" });
            }

            if (!order.SapCreatedTime.HasValue)
            {
                output.Add(new ImportOutput { RowId = rowId, ErrorMsg = $"SAP创建时间不能为空" });
            }

            rowId++;
        }

        // 有错误则直接返回
        if (output.Any())
        {
            return output;
        }

        var soCodes = orders.Select(p => p.SoCode).Distinct().ToList();
        var orderEntities = new List<OrderEntity>();
        var dbOrders = await orderRep.Value.Select.Where(a => soCodes.Contains(a.SoCode)).ToListAsync();
        foreach (var soCode in soCodes)
        {
            var orderEntity = new OrderEntity();
            var orderList = orders.Where(p => p.SoCode == soCode).ToList();
            var orderOne = orderList.FirstOrDefault();
            var dbOrder = dbOrders.FirstOrDefault(a => a.SoCode == soCode);
            if (orderOne != null)
            {
                orderEntity.SoCode = orderOne.SoCode;
                orderEntity.ReferenceCode = orderOne.ReferenceCode;
                orderEntity.PrCode = dbOrder?.PrCode ?? orderOne.ReferenceCode.Split("-")[0];
                // 经销商名称
                orderEntity.SoldCode = orderOne.SoldCode;
                orderEntity.DealerName = dealers.Where(p => p.SoldCode == orderOne.SoldCode).Select(p => p.Name)
                    .FirstOrDefault();
                // 经销商地址
                orderEntity.ShipCode = orderOne.ShipCode;
                orderEntity.DealerAddress = addresses
                    .Where(p => p.SoldCode == orderOne.SoldCode && p.ShipCode == orderOne.ShipCode)
                    .Select(p => p.Address).FirstOrDefault();
                orderEntity.Region = dealers.Where(p => p.SoldCode == orderOne.SoldCode).Select(p => p.Region)
                    .FirstOrDefault();
                orderEntity.Batch = orderOne.Batch;
                orderEntity.Count = orderList.Sum(p => p.Count ?? 0);
                orderEntity.SapCreatedTime = orderOne.SapCreatedTime;
                orderEntity.SapCreatedBy = orderOne.SapCreatedBy;
                orderEntity.DeliveryBlock = orderOne.DeliveryBlock;
                // 判断状态

                if (orderList.All(a => a.CancelReason.IsNullOrEmpty()))
                {
                    orderEntity.Status = OrderStatus.Deleted;
                }
                else if (orderOne.SapCreatedTime != null)
                {
                    orderEntity.Status = OrderStatus.Created;
                    orderEntity.HasWarning = !string.IsNullOrWhiteSpace(orderOne.DeliveryBlock);
                }
            }

            orderEntity.Items = orderList.Select(p => new OrderItemEntity
            {
                OrderId = orderEntity.Id,
                Specification = products.Where(o => o.Code == p.ProductCode).Select(o => o.Specification).FirstOrDefault(),
                Unit = products.Where(o => o.Code == p.ProductCode).Select(o => o.Unit).FirstOrDefault(),
                ProductName = products.Where(o => o.Code == p.ProductCode).Select(o => o.Name).FirstOrDefault(),
                ProductCode = p.ProductCode,
                BoxSize = products.Where(o => o.Code == p.ProductCode).Select(o => o.BoxSize).FirstOrDefault(),
                Count = p.Count ?? 0,
                TotalAmount = p.TotalAmount ?? 0,
                CancelReason = p.CancelReason
            }).ToList();
            orderEntities.Add(orderEntity);
        }

        orderRep.Value.DbContextOptions.EnableCascadeSave = true;

        await orderRep.Value.DeleteCascadeByDatabaseAsync(o => soCodes.Contains(o.SoCode));
        await orderRep.Value.InsertAsync(orderEntities);

        return output;

    }

    /// <summary>
    /// 导出订单
    /// </summary>
    /// <returns></returns>
    public async Task<FileContentResult> ExportAsync(OrderGetPageInput input)
    {
        var soldCodes = new List<string>();
        if (!User.RoleNames.Contains(RoleNames.ExHub))
        {
            soldCodes.Add(User.SoldCode);
            soldCodes.AddRange(await dealerService.Value.GetCurrentUserSoldCodesAsync());
        }
        var orders = await orderRep.Value.Select
            .IncludeMany(a => a.Items)
            .Include(a => a.Contract)
            .IncludeMany(a => a.Invoices)
            .Include(a => a.Express)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Key), a => a.DealerName.Contains(input.Key) || a.SoCode.Contains(input.Key) || a.PrCode.Contains(input.Key))
            .WhereIf(input.Status != null, a => a.Status == input.Status)
            .WhereIf(soldCodes.Any(), a => soldCodes.Contains(a.SoldCode))
            .WhereIf(input.SapCreatedTimeFrom != null, a => a.SapCreatedTime.Value.Date >= input.SapCreatedTimeFrom.Value.Date)
            .WhereIf(input.SapCreatedTimeTo != null, a => a.SapCreatedTime.Value.Date <= input.SapCreatedTimeFrom.Value.AddDays(1))
            .WhereIf(!string.IsNullOrWhiteSpace(input.Region), a => a.Region == input.Region)
            .WhereIf(!string.IsNullOrWhiteSpace(input.CreatedUserName), a => a.CreatedUserName.Contains(input.CreatedUserName))
            .OrderByDescending(a => a.Id)
            .ToListAsync();

        var list = new List<OrderExportOutput>();
        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                if (!order.Invoices.Any())
                {
                    order.Invoices = new List<InvoiceEntity>()
                    {
                        new()
                        {
                            Id = 0,
                            SoCode = string.Empty,
                            No = string.Empty,
                            Date = null,
                            Billing = string.Empty,
                        }
                    };
                }
                foreach (var invoice in order.Invoices)
                {
                    var dto = new OrderExportOutput
                    {
                        PrCode = order.PrCode,
                        SoCode = order.SoCode,
                        DealerName = order.DealerName,
                        OrderCount = order.Items.Count,
                        Status = order.Status.ToDescriptionOrString(),
                        SapCreatedTime = order.SapCreatedTime.ToString(),
                        Region = order.Region,
                        SapCreatedUser = order.CreatedUserName,
                        SoldCode = order.SoldCode,
                        ShipCode = order.ShipCode,
                        DealerAddress = order.DealerAddress,
                        ReferenceCode = order.ReferenceCode,
                        DeliveryBlock = order.DeliveryBlock,
                        Amount = item.TotalAmount,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Specification = item.Specification,
                        Unit = item.Unit,
                        Count = item.Count.ToString(),
                        No = order.Express?.DeliveryNo,
                        ExpectedArrivalTime = order.Express?.ExpectedArrivalTime.ToString(),
                        ReceivedTime = order.Express?.ReceivedTime.ToString(),
                        DriverInfo = order.Express?.DriverName,
                        DeliveryWarehouse = order.Express?.Warehouse,
                        TransportMethod = order.Express?.TransportMethod,
                        Company = order.Express?.Company,
                        ReleasedTime = order.Express?.ReleasedTime.ToString(),
                        InvoiceNo = invoice.No,
                        InvoiceTime = invoice?.Date.ToString(),
                        PgiTime = order.Express?.PgiTime.ToString(),
                        GtsNo = invoice?.Billing
                    };
                    list.Add(dto);
                }

            }

        }

        var mapper = new Mapper();
        MemoryStream stream = new MemoryStream();

        mapper.Save(stream, list, "Sheet1", false);

        return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"Order_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        };
    }

    /// <summary>
    /// 获取发票信息
    /// </summary>
    /// <param name="orderNos"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<MyQueryInvoiceResultResponse> GetInvoices(List<string> orderNos)
    {
        var result = await nuoNuoSdk.Value.ExecuteAsync<MyQueryInvoiceResultRequest, MyQueryInvoiceResultResponse>(
            new MyQueryInvoiceResultRequest()
            {
                OrderNos = orderNos
            });

        return result;
    }

    /// <summary>
    /// 导入发票数据
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<List<ImportOutput>> ImportInvoiceAsync(IFormFile formFile)
    {
        // 读取Excel字段
        var mapper = new Mapper(formFile.OpenReadStream())
        {
            TrimSpaces = TrimSpacesType.Both,
            SkipBlankRows = true,
            FirstRowIndex = 3
        };

        mapper.Map<InvoiceEntity>("Sales Order Number", a => a.SoCode)
            .Map<InvoiceEntity>("Order Type", a => a.OrderType)
            .Map<InvoiceEntity>("VAT Invoice Number", a => a.No)
            .Map<InvoiceEntity>("VAT Invoice Date", a => a.Date)
            .Map<InvoiceEntity>("Billing document", a => a.Billing);
        var invoices = mapper.Take<InvoiceEntity>().Select(i => i.Value).Where(a => a.OrderType == "ZOR").ToList();

        if (!invoices.Any())
        {
            throw ResultOutput.Exception("数据不能为空");
        }

        var outputs = new List<ImportOutput>();
        var rowId = 5;
        var orders = await orderRep.Value.Select.Where(a => invoices.Any(b => b.SoCode == a.SoCode)).ToListAsync();


        #region 校验

        foreach (var invoice in invoices)
        {
            if (invoice.Billing.IsNullOrEmpty())
            {
                outputs.Add(new ImportOutput()
                {
                    RowId = rowId,
                    ErrorMsg = "Billing Document 必填"
                });
            }

            if (invoice.No.IsNullOrEmpty())
            {
                outputs.Add(new ImportOutput()
                {
                    RowId = rowId,
                    ErrorMsg = "VAT Invoice Number 不能为空"
                });
            }

            if (invoice.SoCode.IsNullOrEmpty())
            {
                outputs.Add(new ImportOutput()
                {
                    RowId = rowId,
                    ErrorMsg = "Sales Order Number 不能为空"
                });
            }
            else if (!orders.Exists(a => a.SoCode == invoice.SoCode))
            {
                outputs.Add(new ImportOutput()
                {
                    RowId = rowId,
                    ErrorMsg = "系统中不存在 Sales Order Number"
                });
            }

            if (invoice.Date == null)
            {
                outputs.Add(new ImportOutput()
                {
                    RowId = rowId,
                    ErrorMsg = "VAT Invoice Date 不能为空/格式有误"
                });
            }

            rowId++;
        }

        if (outputs.Any())
        {
            return outputs;
        }

        #endregion


        await invoiceRep.Value.Select.Where(a => invoices.Any(b => b.SoCode == a.SoCode)).ToDelete()
            .ExecuteAffrowsAsync();

        var addInvoices = invoices.GroupBy(a => a.SoCode).Select(a => a.First()).ToList();

        var list = addInvoices.Where(a => !a.Billing.Contains("&")).ToList();

        foreach (var add in addInvoices.Where(a => a.Billing.Contains("&")))
        {
            var arr = add.Billing.Split("&");
            foreach (var s in arr)
            {
                list.Add(new InvoiceEntity
                {
                    Billing = s.Trim(),
                    Date = add.Date,
                    No = add.No,
                    SoCode = add.SoCode,
                    OrderType = add.OrderType
                });
            }
        }
        await invoiceRep.Value.InsertAsync(list);
        return outputs;
    }
}


