using Application.Dtos.Users;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email); //devuelve usuario o nulo.
        Task<User?> GetByIdWithRolesAsync(int id);  
        Task<User?> GetByEmailWithRolesAsync(string email);

        Task<IEnumerable<User>> GetAllWithRolesAsync();

        Task<IEnumerable<User>> GetDeletionRequestsAsync();
        Task<IEnumerable<User>> GetUsersMarkedForDeletionAsync(DateTime currentDate);
    }
}
