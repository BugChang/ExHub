using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Dealer
{
    /// <summary>
    /// 经销商地址
    /// </summary>
    [Table(Name = "ad_dealer_address")]
    public class DealerAddressEntity : Entity
    {
        /// <summary>
        /// SoldToCode
        /// </summary>
        public string SoldCode { get; set; }


        /// <summary>
        /// ShipCode
        /// </summary>
        public string ShipCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 收件人联系方式
        /// </summary>
        public string ReceiverMobile { get; set; }
    }
}
