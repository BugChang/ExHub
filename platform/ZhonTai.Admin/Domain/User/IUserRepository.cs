using System.Collections.Generic;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Repositories;

namespace ZhonTai.Admin.Domain.User;

public interface IUserRepository : IRepositoryBase<UserEntity>
{
    Task<List<UserEntity>> GetListByRoleNameAsync(string roleName);
}