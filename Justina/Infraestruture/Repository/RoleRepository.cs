using Application.Interface.Repository;
using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestruture.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(JustinaDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UserHasRoleAsync(int userId, string roleName)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                               ur.Role.Name == roleName &&
                               ur.Role.IsActive);
        }

        public async Task<IEnumerable<Role>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<int> GetUserCountByRoleIdAsync(int roleId)
        {
            return await _context.UserRoles
                .CountAsync(ur => ur.RoleId == roleId);
        }

        public async Task<Role?> GetRoleWithUsersAsync(int roleId)
        {
            return await _dbSet
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }
    }
}
