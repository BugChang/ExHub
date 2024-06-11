using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Job
{
    public interface IJobService
    {
        /// <summary>
        /// 同步SINO物流数据
        /// </summary>
        /// <returns></returns>
        Task<string> SyncSinoExpressAsync();

        /// <summary>
        /// 同步SINO产品数据
        /// </summary>
        /// <returns></returns>
        Task<string> SyncSinoProductAsync();

        /// <summary>
        /// 经销商证照到期提醒
        /// </summary>
        /// <returns></returns>
        Task<string> SendLicenseExpirationMessageAsync();

        /// <summary>
        /// 同步客商数据
        /// </summary>
        /// <returns></returns>
        Task<string> SyncDealerAddressAsync();
    }
}
