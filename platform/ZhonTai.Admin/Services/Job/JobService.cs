using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.SshNet;
using System.Threading.Tasks;
using LogicExtensions;
using Microsoft.Extensions.Configuration;
using System.Xml;
using ZhonTai.Admin.Domain.Dealer;
using ZhonTai.Admin.Domain.Express;
using ZhonTai.Admin.Domain.Product;
using ZhonTai.Admin.Domain.User;
using ZhonTai.Admin.Services.User;
using ZhonTai.DynamicApi;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Domain.Message;
using Qiniu.Storage;

namespace ZhonTai.Admin.Services.Job
{
    public class JobService(IConfiguration configuration,
            IExpressRepository expressRepository,
            IProductRepository productRepository,
            IDealerLicenseRepository dealerLicenseRepository,
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IDealerAddressMapRepository dealerAddressMapRepository,
            IDealerAddressRepository dealerAddressRepository)
        : BaseService, IJobService
    {

        private SftpClient _sftpClient;
        private readonly string[] _allowWarehouses = { "CN51", "CN61" };

        private void InitSftpClient()
        {
            var host = configuration.GetSection("Sino:Host").Value;
            var port = configuration.GetSection("Sino:Port").Value.ToInt32();
            var username = configuration.GetSection("Sino:Username").Value;
            var password = configuration.GetSection("Sino:Password").Value;
            _sftpClient = new SftpClient(host, port, username, password);
        }

        /// <summary>
        /// 同步赛飞物流数据
        /// </summary>
        /// <returns></returns>
        public async Task<string> SyncSinoExpressAsync()
        {
            InitSftpClient();
            var sb = new StringBuilder();
            try
            {
                _sftpClient.Connect();

                sb.AppendLine(await SyncOrdAsync());

                sb.AppendLine(await SyncPgiAsync());

                sb.AppendLine(await SyncPodAsync());

                sb.AppendLine(await SyncTmsAsync());
            }
            finally
            {
                _sftpClient.Disconnect();
            }

            return sb.ToString();
        }

        /// <summary>
        /// 同步产品
        /// </summary>
        /// <returns></returns>
        public async Task<string> SyncSinoProductAsync()
        {
            // 处理产品
            var list = _sftpClient.ListDirectory("/Quality/UP/Material");
            var files = list.Where(f => !f.IsDirectory && f.FullName.Contains(".xml")).Select(f => f.FullName).ToList();
            var successCount = 0;
            foreach (var file in files)
            {
                var xml = _sftpClient.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;
                // 产品代码
                var productCodeNodes = root.SelectNodes("/ZCLFMAS_MAT/IDOC/E1OCLFM/OBJEK");


                var productCode = productCodeNodes[0]?.InnerText;
                var product = await productRepository.Select.Where(a => a.Code == productCode).ToOneAsync() ?? new ProductEntity
                {
                    Code = productCode
                };

                var commonNodes = root.SelectNodes("/ZCLFMAS_MAT/IDOC/E1OCLFM/E1AUSPM");

                Dictionary<string, string> properties = new Dictionary<string, string>();

                // 遍历 commonNodes 一次，将属性名和对应的值存储到字典中
                foreach (XmlNode node in commonNodes)
                {
                    string attributeName = node.SelectSingleNode("ATNAM")?.InnerText;
                    string attributeValue = node.SelectSingleNode("ATWRT")?.InnerText;
                    properties[attributeName] = attributeValue;
                }

                // 使用字典中的值来设置对应的属性
                if (properties.ContainsKey("GENERIC_NAME_GSP"))
                {
                    product.Name = properties["GENERIC_NAME_GSP"];
                }

                if (properties.ContainsKey("BRANDNAME_GSP"))
                {
                    product.Name += $"({properties["BRANDNAME_GSP"]})";
                }

                if (properties.ContainsKey("CASE_QUANTITY_GSP"))
                {
                    product.BoxSize = Convert.ToInt32(properties["CASE_QUANTITY_GSP"]);
                }

                if (properties.ContainsKey("STRENGTH_GSP"))
                {
                    product.Specification = properties["STRENGTH_GSP"];
                }

                if (properties.ContainsKey("MIN_PACK_UNIT_GSP"))
                {
                    product.Unit = properties["UNIT_GSP"];
                }

                await productRepository.InsertOrUpdateAsync(product);
                successCount++;
                _sftpClient.DeleteFile(file);

            }
            return $"同步产品：{successCount}/{files.Count}";
        }

        /// <summary>
        /// 同步ORD
        /// </summary>
        /// <returns></returns>
        private async Task<string> SyncOrdAsync()
        {
            var list = _sftpClient.ListDirectory("/Quality/UP/SHPORD");
            var files = list.Where(f => !f.IsDirectory && f.FullName.Contains(".xml")).Select(f => f.FullName).ToList();
            var successCount = 0;
            foreach (var file in files)
            {
                var xml = _sftpClient.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;
                if (root == null)
                {
                    continue;
                }
                var soCodeNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/E1EDL24/E1EDL43/BELNR");
                if (soCodeNodes == null || soCodeNodes.Count == 0)
                {
                    continue;
                }

                var lfart = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/E1EDL21/LFART");
                if (lfart[0].InnerText != "ZLF")
                {
                    continue;
                }

                // 订单号
                var soCode = soCodeNodes[0].InnerText;
                var express = await expressRepository.Select.Where(a => a.SoCode == soCode).ToOneAsync() ?? new ExpressEntity()
                {
                    SoCode = soCode
                };

                // 运单号
                var deliveryNoNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/VBELN");
                var deliveryNo = deliveryNoNodes[0].InnerText;
                express.DeliveryNo = deliveryNo;

                // 发货仓
                var warehouseNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/E1EDL24/WERKS");
                var warehouse = warehouseNodes[0].InnerText;
                if (!_allowWarehouses.Contains(warehouse))
                {
                    continue;
                }
                express.Warehouse = warehouse;

                // 发运批号
                var batchNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/E1EDL24");
                express.ExpressBatch = new List<ExpressBatchEntity>();
                foreach (XmlNode batch in batchNodes)
                {
                    DateTime.TryParseExact(batch.SelectSingleNode("VFDAT").InnerText, "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out var validity);
                    var expressBatch = new ExpressBatchEntity
                    {
                        BatchNo = batch.SelectSingleNode("CHARG").InnerText,
                        ValidityPeriod = validity,
                        Count = batch.SelectSingleNode("LFIMG").InnerText
                    };
                    express.ExpressBatch.Add(expressBatch);
                }

                // 释放时间
                var releaseDateNodes = root.SelectNodes("/DELVRY05/IDOC/EDI_DC40/CREDAT");
                var releaseDate = releaseDateNodes[0].InnerText;
                var releaseTimeNodes = root.SelectNodes("/DELVRY05/IDOC/EDI_DC40/CRETIM");
                var releaseTime = releaseTimeNodes[0].InnerText;

                DateTime.TryParseExact(releaseDate + releaseTime, "yyyyMMddHHmmss", null,
                    System.Globalization.DateTimeStyles.None, out var releaseDateTime);
                express.ReleasedTime = releaseDateTime;
                // 入库
                await expressRepository.InsertOrUpdateAsync(express);
                successCount++;
                // 删除文件
                _sftpClient.DeleteFile(file);
            }

            return $"同步ORD：{successCount}/{files.Count}";
        }

        /// <summary>
        /// 同步PGI
        /// </summary>
        /// <returns></returns>
        private async Task<string> SyncPgiAsync()
        {
            var list = _sftpClient.ListDirectory("/Quality/UP/SHPPGI");
            var files = list.Where(f => !f.IsDirectory && f.FullName.Contains(".xml")).Select(f => f.FullName).ToList();
            var successCount = 0;
            foreach (var file in files)
            {
                var xml = _sftpClient.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;

                // 运单号
                var deliveryNoNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/VBELN");
                var deliveryNo = deliveryNoNodes[0].InnerText;
                var express = await expressRepository.Select.Where(a => a.DeliveryNo == deliveryNo).ToOneAsync();
                if (express != null)
                {
                    var pgiNoNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/E1EDT13/NTANF");
                    DateTime.TryParseExact(pgiNoNodes[0].InnerText, "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out var pgiTime);
                    express.PgiTime = pgiTime;
                    await expressRepository.UpdateAsync(express);
                    successCount++;
                    _sftpClient.DeleteFile(file);
                }
            }
            return $"同步PGI：{successCount}/{files.Count}";
        }

        /// <summary>
        /// 同步POD
        /// </summary>
        /// <returns></returns>
        private async Task<string> SyncPodAsync()
        {
            var list = _sftpClient.ListDirectory("/Quality/UP/SHPPOD");
            var files = list.Where(f => !f.IsDirectory && f.FullName.Contains(".xml")).Select(f => f.FullName).ToList();
            var successCount = 0;
            foreach (var file in files)
            {
                var xml = _sftpClient.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;

                // 运单号
                var deliveryNoNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/VBELN");
                var deliveryNo = deliveryNoNodes[0].InnerText;
                var express = await expressRepository.Select.Where(a => a.DeliveryNo == deliveryNo).ToOneAsync();
                if (express != null)
                {
                    var receiveNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/PODAT");
                    DateTime.TryParseExact(receiveNodes[0].InnerText, "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out var receiveTime);
                    express.ReleasedTime = receiveTime;
                    await expressRepository.UpdateAsync(express);
                    successCount++;
                    _sftpClient.DeleteFile(file);
                }
            }
            return $"同步POD：{successCount}/{files.Count}";
        }

        /// <summary>
        /// 同步TMS
        /// </summary>
        /// <returns></returns>
        private async Task<string> SyncTmsAsync()
        {
            var list = _sftpClient.ListDirectory("/Quality/UP/TMS");
            var files = list.Where(f => !f.IsDirectory && f.FullName.Contains(".xml")).Select(f => f.FullName).ToList();
            var successCount = 0;
            foreach (var file in files)
            {
                var xml = _sftpClient.ReadAllText(file);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;

                // 运单号
                var deliveryNoNodes = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/BILLNO");
                var deliveryNo = deliveryNoNodes[0].InnerText;
                var express = await expressRepository.Select.Where(a => a.DeliveryNo == deliveryNo).ToOneAsync();
                if (express != null)
                {
                    express.Company = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/CARRIER")[0].InnerText;
                    express.DriverName = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/DRIVER")[0].InnerText;
                    express.DriverPhone = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/PHONE")[0].InnerText;
                    express.TransportMethod = root.SelectNodes("/DELVRY05/IDOC/E1EDL20/TRANSMODE")[0].InnerText;
                    DateTime.TryParseExact(root.SelectNodes("/DELVRY05/IDOC/E1EDL20/PLANARRIVETIME")[0].InnerText, "yyyy-MM-dd HH:mm:ss", null,
                        System.Globalization.DateTimeStyles.None, out var expectedArrivalTime);
                    express.ExpectedArrivalTime = expectedArrivalTime;
                    await expressRepository.UpdateAsync(express);
                    successCount++;
                    _sftpClient.DeleteFile(file);
                }
            }
            return $"同步TMS：{successCount}/{files.Count}";
        }

        /// <summary>
        /// 经销商证照到期提醒
        /// </summary>
        /// <returns></returns>
        public async Task<string> SendLicenseExpirationMessageAsync()
        {
            // 查询90天以内到期的经销商

            var licenses = await dealerLicenseRepository.Select
                .Include(a => a.Dealer)
                .Where(a => a.ExpirationDate.Value.Date <= DateTime.Now.AddDays(90).Date)
                .ToListAsync();

            var messages = new List<MessageEntity>();
            var exHubUsers = await userRepository.GetListByRoleNameAsync(RoleNames.ExHub);
            foreach (var license in licenses)
            {
                var receiveUsers = new List<long>();
                receiveUsers.AddRange(exHubUsers.Select(a => a.Id));
                receiveUsers.Add(license.Dealer.BizUserId);
                receiveUsers.Add(license.Dealer.RegionManagerUserId);
                foreach (var receiveUser in receiveUsers)
                {
                    messages.Add(new MessageEntity
                    {
                        Title = "经销商证照到期提醒",
                        Content = $"（{license.Dealer.Name}）的（{license.Name}）资质即将在（{(license.ExpirationDate.Value - DateTime.Now).Days}）天到期，请及时更新信息。",
                        CreatedTime = DateTime.Now,
                        ReceiveUserId = receiveUser
                    });
                }
            }

            if (messages.Any())
            {
                await messageRepository.InsertAsync(messages);
            }

            return $"新增消息：{messages.Count}";
        }

        /// <summary>
        /// 同步客商数据
        /// </summary>
        /// <returns></returns>
        public async Task<string> SyncDealerAddressAsync()
        {
            try
            {
                InitSftpClient();
                _sftpClient.Connect();
                var list = _sftpClient.ListDirectory("/Quality/UP/Customer");
                var files = list.Where(f => !f.IsDirectory && f.FullName.Contains(".xml")).Select(f => f.FullName).ToList();
                var successCount = 0;

                foreach (var file in files)
                {
                    var xml = _sftpClient.ReadAllText(file);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var root = doc.DocumentElement;

                    // 客商编码
                    var code = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/KUNNR")[0].InnerText;
                    var region = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/REGION")?[0]?.InnerText ?? "";
                    var city = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/CITY")?[0]?.InnerText ?? "";
                    var street = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/STREET")?[0]?.InnerText ?? "";
                    var houseNo = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/HOUSE_NO")?[0]?.InnerText ?? "";
                    var houseNo2 = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/HOUSE_NO2")?[0]?.InnerText ?? "";
                    var houseNo3 = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/HOUSE_NO3")?[0]?.InnerText ?? "";
                    var strSuppl1 = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/STR_SUPPL1")?[0]?.InnerText ?? "";
                    var strSuppl2 = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/STR_SUPPL2")?[0]?.InnerText ?? "";
                    var strSuppl3 = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/ZE1ADRMAS/ZE1BPAD1VL/STR_SUPPL3")?[0]?.InnerText ?? "";
                    var type = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/KTOKD")?[0]?.InnerText;
                    if (type == "ZC01")
                    {
                        var mapList = new List<DealerAddressMapEntity>();
                        var shipCodesNode = root.SelectNodes("/ZDEB_ADR_CLF02/IDOC/E1KNA1M/E1KNVVM/E1KNVPM");
                        foreach (XmlNode shipCode in shipCodesNode)
                        {
                            var par = shipCode.SelectSingleNode("PARVW").InnerText;
                            if (par == "WE")
                            {
                                mapList.Add(new DealerAddressMapEntity
                                {
                                    SoldCode = code,
                                    ShipCode = shipCode.SelectSingleNode("KUNN2").InnerText
                                });
                            }
                        }

                        await dealerAddressMapRepository.Select.Where(a => a.SoldCode == code).ToDelete()
                            .ExecuteAffrowsAsync();
                        await dealerAddressMapRepository.InsertAsync(mapList);
                    }

                    var address = await dealerAddressRepository.Select.Where(a => a.ShipCode == code).ToOneAsync() ?? new DealerAddressEntity
                    {
                        ShipCode = code,
                        Address = region + city + street + houseNo + houseNo2 + houseNo3 + strSuppl1 + strSuppl2 +
                                  strSuppl3
                    };

                    await dealerAddressRepository.InsertOrUpdateAsync(address);
                    successCount++;
                    _sftpClient.DeleteFile(file);
                }

                // 刷新关系
                if (successCount > 0)
                {
                    await dealerAddressRepository.Orm.Update<DealerAddressEntity>()
                        .Set(a => a.SoldCode, "")
                        .Where(a => true).ExecuteAffrowsAsync();

                    await dealerAddressRepository.Orm.Update<DealerAddressEntity>()
                        .Join<DealerAddressMapEntity>((a, b) => a.ShipCode == b.ShipCode)
                        .Set((a, b) => a.SoldCode == b.SoldCode)
                        .ExecuteAffrowsAsync();
                }

                return $"同步客商数据：{successCount}/{files.Count}";
            }
            finally
            {
                _sftpClient.Disconnect();
            }
        }
    }
}
