using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Core.Enums
{
    public enum EffectiveStatus
    {
        [Description("未生效")]
        NotActive = 0,
        [Description("生效中")]
        Active = 1,
        [Description("已失效")]
        InActive = 2,
    }
}
