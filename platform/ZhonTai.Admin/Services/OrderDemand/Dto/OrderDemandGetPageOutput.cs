using System;
using System.ComponentModel.DataAnnotations;

namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandGetPageOutput : OrderDemandDto
    {
        /// <summary>
        /// PR 总数量
        /// </summary>
        public int PrTotalAmount { get; set; } = 0;

        /// <summary>
        /// SO 总数量
        /// </summary>
        public int SoTotalAmount { get; set; } = 0;

        /// <summary>
        /// 区域
        /// </summary>
        [MaxLength(20)]
        public string Region { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedUserName { get; set; }


        /// <summary>
        /// 更新人真实姓名
        /// </summary>
        public string ModifiedUserRealName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }

    }
}
