using FreeSql.DataAnnotations;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.Dealer;

namespace ZhonTai.Admin.Services.Dealer.Dto
{
    public class DealerImportDto
    {
        /// <summary>
        /// SoldToCode
        /// </summary>
        public string SoldCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 经销商类别
        /// </summary>
        public string TypeStr { get; set; }

        /// <summary>
        /// 经销商类别
        /// </summary>
        public DealerType Type
        {
            get => GetType(TypeStr);
            set { }
        }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// CSL商务
        /// </summary>
        public long BizUserId { get; set; }

        /// <summary>
        /// CSL商务邮箱
        /// </summary>
        public string BizUserEmail { get; set; }

        /// <summary>
        /// ExHub负责人
        /// </summary>
        public long ExHubUserId { get; set; }

        /// <summary>
        /// ExHub负责人邮箱
        /// </summary>
        public string ExHubUserEmail { get; set; }

        /// <summary>
        /// 大区经理
        /// </summary>
        public long RegionManagerUserId { get; set; }

        /// <summary>
        /// 大区经理邮箱
        /// </summary>
        public string RegionManagerEmail { get; set; }


        /// <summary>
        /// 经销商状态
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        /// 经销商状态
        /// </summary>
        public DealerStatus Status
        {
            get => GetStatus(StatusStr);
            set { }
        }


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

        private DealerType GetType(string type)
        {
            if(type == "一般经销商")
            {
                return DealerType.Normal;
            }

            if (type == "重点经销商")
            {
                return DealerType.Emphasis;
            }

            return DealerType.Normal;
        }


        private DealerStatus GetStatus(string status)
        {
            if (status == "Inactive")
            {
                return DealerStatus.Inactive;
            }

            if (status == "Active")
            {
                return DealerStatus.Active;
            }

            return DealerStatus.Active;
        }
    }
}
