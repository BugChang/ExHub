namespace ZhonTai.Admin.Core.Consts
{
    /// <summary>
    /// 任务常量
    /// </summary>
    public static partial class TaskNames
    {
        /// <summary>
        /// 同步SINO物流数据
        /// </summary>
        public const string SinoExpressSyncTask = "SinoExpressSyncTask";

        /// <summary>
        /// 同步SINO产品数据
        /// </summary>
        public const string SinoProductSyncTask = "SinoProductSyncTask";

        /// <summary>
        /// 经销商证照到期提醒任务
        /// </summary>
        public const string SendLicenseExpirationMessageTask = "SendLicenseExpirationMessageTask";

        /// <summary>
        /// 同步客商数据
        /// </summary>
        public const string SyncDealerAddressTask = "SyncDealerAddressTask";
    }
}
