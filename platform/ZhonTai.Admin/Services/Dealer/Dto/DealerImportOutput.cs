﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhonTai.Admin.Services.Dealer.Dto
{
    public class DealerImportOutput
    {
        /// <summary>
        /// 错误行号
        /// </summary>
        public int RowId { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
