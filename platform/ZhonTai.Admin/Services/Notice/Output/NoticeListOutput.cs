using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Common.Extensions;

namespace ZhonTai.Admin.Services.Notice.Output
{
    public class NoticeListOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 共享范围
        /// </summary>
        public SharedScope Scope { get; set; }

        /// <summary>
        /// 共享范文字
        /// </summary>
        public string ScopeStr
        {
            get
            {
                return Scope.ToDescription();
            }
        }

        /// <summary>
        /// 发布部门
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 创建人用户名
        /// </summary>
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>

        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 最新变更时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        /// 当前登录用户是否已看过本篇公告
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatedUserRealName { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string ModifiedUserName { get; set; }


        /// <summary>
        /// 更新人真实姓名
        /// </summary>
        public string ModifiedUserRealName { get; set; }

    }
}
