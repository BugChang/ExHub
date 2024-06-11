using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Db.Transaction;
using ZhonTai.Admin.Domain.User;

namespace ZhonTai.Admin.Repositories;

public class UserRepository : AdminRepositoryBase<UserEntity>, IUserRepository
{
    public UserRepository(UnitOfWorkManagerCloud muowm) : base(muowm)
    {

    }

    public Task<List<UserEntity>> GetListByRoleNameAsync(string roleName)
    {
        return Select.Where(a => a.Roles.Any(r => r.Name == roleName)).ToListAsync();
    }
}