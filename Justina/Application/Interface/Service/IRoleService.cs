using Application.Dtos.Roles;
using Application.Interface.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Service
{
    public interface IRoleService
    {
        // CRUD Básico
        Task<IResult<RoleReadDto>> CreateRoleAsync(RoleCreateDto createDto);
        Task<IResult<RoleReadDto>> GetRoleByIdAsync(int id);
        Task<IResult<IEnumerable<RoleReadDto>>> GetAllRolesAsync();
        Task<IResult<RoleReadDto>> UpdateRoleAsync(int id, RoleUpdateDto updateDto);
        Task<IResult<bool>> DeleteRoleAsync(int id);

        // Asignación de roles
        Task<IResult<bool>> AssignRoleToUserAsync(AssignRoleDto assignDto);
        Task<IResult<bool>> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IResult<IEnumerable<RoleReadDto>>> GetRolesByUserIdAsync(int userId);

        // Utilitarios
        Task<IResult<bool>> RoleExistsAsync(string roleName);
        Task<IResult<int>> GetUserCountByRoleIdAsync(int roleId);
    }
}
