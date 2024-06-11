namespace ZhonTai.Admin.Services.Message.Dto
{
    public class MessageAddInput
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public string ReceiveUserName { get; set; }
    }
}
