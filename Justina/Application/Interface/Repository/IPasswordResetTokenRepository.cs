using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
        Task<IEnumerable<PasswordResetToken>> GetTokensByUserIdAsync(int userId);
        Task InvalidateOldTokensAsync(int userId);
    }
}
