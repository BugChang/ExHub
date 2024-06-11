using System.ComponentModel.DataAnnotations;

namespace ZhonTai.Admin.Services.Product.Dto
{
    public class ProductUpdateInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required(ErrorMessage = "请选择产品")]
        public long Id { get; set; }

        /// <summary>
        /// 常用名
        /// </summary>
        public string CommonName { get; set; }

        /// <summary>
        /// 箱规
        /// </summary>
        public int BoxSize { get; set; }
    }
}
