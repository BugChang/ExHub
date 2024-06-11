using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Domain.SerialNo;

namespace ZhonTai.Admin.Services.SerialNo
{
    public interface ISerialNoService
    {
        public Task<string> GetSerialNoAsync(SerialNoType serialNoType);
    }
}
