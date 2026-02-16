using Application.Interface.Repository;
using Domain.Models;
using Infraestruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestruture.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(JustinaDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdWithRolesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailWithRolesAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllWithRolesAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetDeletionRequestsAsync()
        {
            return await _context.Users
                .Where(u => u.DeletionRequested && u.ScheduledDeletionDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersMarkedForDeletionAsync(DateTime currentDate)
        {
            return await _context.Users
                .Where(u => u.DeletionRequested && u.ScheduledDeletionDate <= currentDate)
                .ToListAsync();
        }
    }
}
