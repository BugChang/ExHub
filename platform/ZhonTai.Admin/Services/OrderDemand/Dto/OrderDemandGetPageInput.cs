using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Domain.OrderDemand;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    /// <summary>
    /// 查询订单需求条件组
    /// </summary>
    public class OrderDemandGetPageInput
    {
        /// <summary>
        /// 经销商名称/PR Code
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderDemandStatus? Status { get; set; }

        /// <summary>
        /// 是否加急
        /// </summary>
        public bool? IsUrgent { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 显示差异订单
        /// </summary>
        public bool ShowDifference { get; set; }

        /// <summary>
        /// 更新开始时间
        /// </summary>
        public DateTime? CreatedTimeFrom { get; set;}

        /// <summary>
        /// 更新结束时间
        /// </summary>
        public DateTime? CreatedTimeTo { get; set; }

    }
}
