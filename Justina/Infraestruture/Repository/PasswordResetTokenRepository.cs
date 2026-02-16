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
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(JustinaDbContext context) : base(context)
        {
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _dbSet
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token
                    && t.ExpiryDate > DateTime.UtcNow
                    && !t.IsUsed);
        }

        public async Task<IEnumerable<PasswordResetToken>> GetTokensByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task InvalidateOldTokensAsync(int userId)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && !t.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
            }
        }
    }
}
