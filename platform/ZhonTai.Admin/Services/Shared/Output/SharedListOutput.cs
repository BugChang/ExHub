using System;
using ZhonTai.Admin.Core.Enums;

namespace ZhonTai.Admin.Services.Shared.Output
{
    public class SharedListOutput
    {
        /// <summary>
        /// 文件Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public FileTypeEnum FileType
        {
            get => GetFileType(FileExtension);
            set { }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedUserName { get; set; }

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

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

        private FileTypeEnum GetFileType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return FileTypeEnum.Other;
            }
            switch (extension.ToLower())
            {
                case ".pdf":
                    return FileTypeEnum.PDF;
                case ".doc":
                    return FileTypeEnum.Word;
                case ".docx":
                    return FileTypeEnum.Word;
                case ".xls":
                    return FileTypeEnum.Excel;
                case ".xlsx":
                    return FileTypeEnum.Excel;
                case ".png":
                    return FileTypeEnum.Picture;
                case ".jpg":
                    return FileTypeEnum.Picture;
                case ".jpeg":
                    return FileTypeEnum.Picture;
                case ".webp":
                    return FileTypeEnum.Picture;
                case ".msg":
                    return FileTypeEnum.Email;
                default:
                    return FileTypeEnum.Other;
            }

        }

    }
}
