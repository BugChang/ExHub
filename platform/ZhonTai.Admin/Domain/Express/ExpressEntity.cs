using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Express
{
    /// <summary>
    /// 物流
    /// </summary>
    [Table(Name = "ad_express")]
    public class ExpressEntity : EntityBase
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string SoCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string DeliveryNo { get; set; }

        /// <summary>
        /// 预计到货时间
        /// </summary>
        public DateTime? ExpectedArrivalTime { get; set; }

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? ReceivedTime { get; set; }

        /// <summary>
        /// 司机姓名
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// 司机电话
        /// </summary>
        public string DriverPhone { get; set; }

        /// <summary>
        /// 发货仓
        /// </summary>
        public string Warehouse { get; set; }

        /// <summary>
        /// 运输方式
        /// </summary>
        public string TransportMethod { get; set; }

        /// <summary>
        /// 承运单位
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 释放时间
        /// </summary>
        public DateTime? ReleasedTime { get; set; }

        /// <summary>
        /// PGI 时间
        /// </summary>
        public DateTime? PgiTime { get; set; }

        /// <summary>
        /// 物流批次
        /// </summary>
        [Navigate(nameof(ExpressBatchEntity.ExpressId))]
        public List<ExpressBatchEntity> ExpressBatch { get; set; }
    }
}
