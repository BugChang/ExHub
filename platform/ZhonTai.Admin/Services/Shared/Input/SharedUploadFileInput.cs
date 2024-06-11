using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Enums;

namespace ZhonTai.Admin.Services.Shared.Input
{
    public class SharedUploadFileInput
    {
        /// <summary>
        /// 共享范围
        /// </summary>
        public SharedScope Scope { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }
    }
}
