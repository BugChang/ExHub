using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Domain.SerialNo;
using ZhonTai.Admin.Services.TaskScheduler;
using ZhonTai.DynamicApi.Attributes;
using ZhonTai.DynamicApi;

namespace ZhonTai.Admin.Services.SerialNo
{

    /// <summary>
    /// 序列号服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class SerialNoService : BaseService, ISerialNoService
    {
        private readonly Lazy<ISerialNoRepository> _serialRep;

        public SerialNoService(Lazy<ISerialNoRepository> serialRep)
        {
            _serialRep = serialRep;
        }

        /// <summary>
        /// 获取序列号
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSerialNoAsync(SerialNoType serialNoType)
        {
            return serialNoType switch
            {
                SerialNoType.OrderDemand => await GetOrderDemandSerialNo(),
                _ => throw new ArgumentOutOfRangeException($"unknown serialNoType : {serialNoType}")
            };
        }

        private async Task<string> GetOrderDemandSerialNo()
        {
            var serialNo = await _serialRep.Value.Select
                .Where(a => a.SerialNoType == SerialNoType.OrderDemand
                            && a.Date == DateTime.Now.Date).ToOneAsync() ?? new SerialNoEntity
                            {
                                Date = DateTime.Now.Date,
                                SerialNoType = SerialNoType.OrderDemand,
                                No = 0
                            };
            serialNo.No++;
            await _serialRep.Value.InsertOrUpdateAsync(serialNo);
            return $"PR{DateTime.Now:yyyyMMdd}{serialNo.No:D3}";
        }
    }
}
