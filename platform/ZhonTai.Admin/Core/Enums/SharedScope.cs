using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Core.Enums
{
    public enum SharedScope
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 0,
        /// <summary>
        /// 外部
        /// </summary>
        [Description("外部")]
        External = 1,
        /// <summary>
        /// 内部
        /// </summary>
        [Description("内部")]
        Internal = 2
    }
}
