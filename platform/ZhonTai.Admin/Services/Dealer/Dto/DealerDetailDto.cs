using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Domain.Dealer;

namespace ZhonTai.Admin.Services.Dealer.Dto
{
    public class DealerDetailDto : DealerDto
    {
        /// <summary>
        /// 地址列表
        /// </summary>
        public List<DealerAddressOutput> Addresses { get; set; }

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

        /// <summary>
        /// 证照列表
        /// </summary>
        public List<DealerLicencesOutput> Licences { get; set; }
    }

    public class DealerLicencesOutput
    {
        public string Name { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? ExpirationDate { get; set; }


        /// <summary>
        /// 效期剩余天数
        /// </summary>
        public int RemainingDays => (ExpirationDate.Value - EffectiveDate.Value).Days;
    }

    public class DealerAddressOutput
    {
        /// <summary>
        /// 
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
