using System;

namespace ZhonTai.Admin.Services.Quota.Dto
{
    public class QuotaGetPageOutput : QuotaDto
    {

        /// <summary>
        /// 行ID
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// 是否有子数据
        /// </summary>
        public bool HasChild { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }


        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
    }
}
