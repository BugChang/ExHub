using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Dealer.Dto
{
    public class DealerAddressListOutput
    {
        public string SoldCode { get; set; }

        public string ShipCode { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// 只读拼接地址和shipcode
        /// </summary>
        public string AddressMergeText { get { return $"{Address}  {ShipCode}";  } }
    }
}
