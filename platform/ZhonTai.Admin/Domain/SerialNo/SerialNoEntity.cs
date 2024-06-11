using System;
using FreeSql.DataAnnotations;
using ZhonTai.Admin.Core.Entities;

namespace ZhonTai.Admin.Domain.SerialNo
{
    [Table(Name = "ad_serial_no")]
    public class SerialNoEntity : EntityBase
    {
        public SerialNoType SerialNoType { get; set; }

        public DateTime Date { get; set; }

        public int No { get; set; }
    }
}
