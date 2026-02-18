using Application.Interface.Repository.UsersRoles;
using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestruture.Repository.UsersRoles
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly JustinaDbContext _context;

        public UserRoleRepository(JustinaDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<UserRole>()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<UserRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UserRole>()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(ur => ur.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UserRole>()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UserRole>()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.RoleId == roleId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<UserRole>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            

            return await _context.Set<UserRole>()
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.Role.Name == roleName)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int userId, int roleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UserRole>()
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        }

        public async Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default)
        {
            if (userRole == null)
                throw new ArgumentNullException(nameof(userRole));

            await _context.Set<UserRole>().AddAsync(userRole, cancellationToken);
        }

        public void Update(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException(nameof(userRole));

            _context.Set<UserRole>().Update(userRole);
        }

        public void Delete(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException(nameof(userRole));

            _context.Set<UserRole>().Remove(userRole);
        }

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
        }
    }
}
