namespace ZhonTai.Admin.Services.Message.Dto
{
    public class MessageGetPageInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title {  get; set; }

        /// <summary>
        /// 已读
        /// </summary>
        public bool? IsRead { get; set; }
    }
}
