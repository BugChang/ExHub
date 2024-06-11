using System;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.Order
{
    [Table(Name = "ad_invoice")]
    public class InvoiceEntity : Entity
    {
        public string SoCode { get; set; }

        public string OrderType { get; set; }

        public string No { get; set; }

        public DateTime? Date { get; set; }

        public string Billing { get; set; }
    }
}
