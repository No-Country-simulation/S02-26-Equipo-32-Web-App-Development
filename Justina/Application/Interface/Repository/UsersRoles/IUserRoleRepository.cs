using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository.UsersRoles
{
    public interface IUserRoleRepository
    {
      
        Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<UserRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        
        Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserRole>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int userId, int roleId, CancellationToken cancellationToken = default);

       
        Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);

        void Update(UserRole userRole);

        void Delete(UserRole userRole);
    }
}
