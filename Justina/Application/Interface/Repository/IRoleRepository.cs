using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<bool> UserHasRoleAsync(int userId, string roleName);
        Task<IEnumerable<Role>> GetAllActiveAsync();
        Task<int> GetUserCountByRoleIdAsync(int roleId);
        Task<Role?> GetRoleWithUsersAsync(int roleId);
    }
}
