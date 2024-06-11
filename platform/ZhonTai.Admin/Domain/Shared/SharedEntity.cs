using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.File;

namespace ZhonTai.Admin.Domain.Shared
{
    [Table(Name = "ad_shared")]
    public class SharedEntity : EntityBase
    {
        /// <summary>
        /// 共享范围
        /// </summary>
        public SharedScope Scope { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }


        [Navigate(nameof(FileId), TempPrimary = nameof(File.Id))]
        public FileEntity File { get; set; }
    }
}
