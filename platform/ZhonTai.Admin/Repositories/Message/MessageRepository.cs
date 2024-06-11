using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.Message;
using ZhonTai.Admin.Domain.Order;

namespace ZhonTai.Admin.Repositories.Message
{
    public class MessageRepository : AdminRepositoryBase<MessageEntity>, IMessageRepository
    {
        public MessageRepository(UnitOfWorkManagerCloud uowm) : base(uowm)
        {
        }
    }
}
