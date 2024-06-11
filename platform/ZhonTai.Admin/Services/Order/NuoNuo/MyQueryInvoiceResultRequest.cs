using NuoNuoSdk.Requests;

namespace ZhonTai.Admin.Services.Order.NuoNuo
{
    public class MyQueryInvoiceResultRequest : QueryInvoiceResultRequest
    {
        public override string Method => "nuonuo.OpeMplatform.queryInvoiceResult";
    }
}
