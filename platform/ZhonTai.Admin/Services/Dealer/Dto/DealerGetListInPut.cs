namespace ZhonTai.Admin.Services.Dealer.Dto
{
    /// <summary>
    /// 经销商客户信息分页查询
    /// </summary>
    public class DealerGetListInPut
    {
        /// <summary>
        /// 客户名称/Sold-to
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// CSL商务
        /// </summary>
        public string BizUserName { get; set; }
    }
}
