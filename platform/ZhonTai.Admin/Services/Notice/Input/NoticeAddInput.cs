using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Enums;

namespace ZhonTai.Admin.Services.Notice.Input
{
    public class NoticeAddInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不可为空")]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required(ErrorMessage = "正文内容不可为空")]
        public string Content { get; set; }

        /// <summary>
        /// 共享范围
        /// </summary>
        public SharedScope Scope { get; set; }

        /// <summary>
        /// 发布部门
        /// </summary>
        [Required(ErrorMessage = "部门不可为空")]
        public string OrgName { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public List<long> FileIds { get; set; } = new List<long>();
    }
}
