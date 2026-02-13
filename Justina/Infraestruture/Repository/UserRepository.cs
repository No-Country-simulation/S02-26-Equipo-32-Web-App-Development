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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(JustinaDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        // 👇 AGREGAR - Obtener usuario con roles
        public async Task<User?> GetByIdWithRolesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // 👇 AGREGAR - Obtener usuario por email con roles
        public async Task<User?> GetByEmailWithRolesAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // ❌ ELIMINAR AddAsync - ya viene de GenericRepository
        // ❌ ELIMINAR UpdateAsync - ya viene de GenericRepository
        // Estos métodos ya existen en GenericRepository
    }
}