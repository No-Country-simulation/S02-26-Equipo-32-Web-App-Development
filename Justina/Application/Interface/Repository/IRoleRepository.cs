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
        /// <summary>
        /// Obtiene un rol por su nombre exacto
        /// </summary>
        Task<Role?> GetByNameAsync(string name);

        /// <summary>
        /// Obtiene todos los roles asignados a un usuario específico
        /// </summary>
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);

        /// <summary>
        /// Verifica si un usuario tiene un rol específico
        /// </summary>
        Task<bool> UserHasRoleAsync(int userId, string roleName);

        /// <summary>
        /// Obtiene todos los roles activos
        /// </summary>
        Task<IEnumerable<Role>> GetAllActiveAsync();

        /// <summary>
        /// Obtiene la cantidad de usuarios que tienen asignado un rol
        /// </summary>
        Task<int> GetUserCountByRoleIdAsync(int roleId);

        /// <summary>
        /// Obtiene un rol con todos sus usuarios asignados
        /// </summary>
        Task<Role?> GetRoleWithUsersAsync(int roleId);
    }
}

