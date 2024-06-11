using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandItemDetail : OrderDemandItemDto
    {

        /// <summary>
        /// 常用名
        /// </summary>
        public string CommonName { get; set; }

    }
}
