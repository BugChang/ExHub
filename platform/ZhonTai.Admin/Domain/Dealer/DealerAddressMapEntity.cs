using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Dealer
{
    /// <summary>
    /// 订单地址映射表
    /// </summary>
    [Table(Name = "ad_dealer_address_map")]
    public class DealerAddressMapEntity : Entity
    {
        /// <summary>
        /// SoldToCode
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// SoldToCode
        /// </summary>
        public string ShipCode { get; set; }
    }
}
