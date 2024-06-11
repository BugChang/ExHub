namespace ZhonTai.Admin.Services.OrderDemand.Dto
{
    public class OrderDemandOrderDto
    {
        public string ProductCode { get; set; }

        public string SoCode { get; set; }

        public string ReferenceCode { get; set; }

        public int Count { get; set; }

        public decimal TotalAmount { get; set; }

    }
}
