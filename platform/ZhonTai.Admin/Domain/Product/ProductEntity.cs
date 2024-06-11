using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Product
{
    /// <summary>
    /// 产品
    /// </summary>
    [Table(Name = "ad_product")]
    public class ProductEntity: EntityBase
    {
        /// <summary>
        /// 代码
        /// </summary>
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [MaxLength(200)]
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(50)]
        public string Unit { get; set; }

        /// <summary>
        /// 常用名
        /// </summary>
        [MaxLength(200)]
        public string CommonName { get; set; }

        /// <summary>
        /// 箱规
        /// </summary>
        public int BoxSize { get; set; }

    }
}
