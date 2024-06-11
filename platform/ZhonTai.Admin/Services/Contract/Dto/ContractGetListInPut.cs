using System;

namespace ZhonTai.Admin.Services.Contract.Dto
{
    public class ContractGetListInPut
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Key { get; set; }


        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? CreatedTimeFrom { get; set; }


        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? CreatedTimeTo { get; set; }
    }
}
