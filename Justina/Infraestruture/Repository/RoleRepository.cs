using Application.Interface.Repository;
using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestruture.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(JustinaDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene un rol por su nombre exacto
        /// </summary>
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        /// <summary>
        /// Obtiene todos los roles asignados a un usuario específico
        /// </summary>
        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si un usuario tiene un rol específico
        /// </summary>
        public async Task<bool> UserHasRoleAsync(int userId, string roleName)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                               ur.Role.Name == roleName &&
                               ur.Role.IsActive);
        }

        /// <summary>
        /// Obtiene todos los roles activos
        /// </summary>
        public async Task<IEnumerable<Role>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene la cantidad de usuarios que tienen asignado un rol
        /// </summary>
        public async Task<int> GetUserCountByRoleIdAsync(int roleId)
        {
            return await _context.UserRoles
                .CountAsync(ur => ur.RoleId == roleId);
        }

        /// <summary>
        /// Obtiene un rol con todos sus usuarios asignados
        /// </summary>
        public async Task<Role?> GetRoleWithUsersAsync(int roleId)
        {
            return await _dbSet
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }
    }
}
