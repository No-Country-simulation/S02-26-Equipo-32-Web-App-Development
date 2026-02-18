using Application.Dtos.UserRole;
using Application.Interface.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Service.UsersRoles
{
    public interface IUserRoleService
    {
        Task<IResult<IEnumerable<UserRoleDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IResult<UserRoleDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<UserRoleDto>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<UserRoleDto>>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<UserRoleDto>>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task<IResult<UserRoleDto>> CreateAsync(CreateUserRoleDto createDto, CancellationToken cancellationToken = default);
        Task<IResult<UserRoleDto>> UpdateAsync(UpdateUserRoleDto updateDto, CancellationToken cancellationToken = default);
        Task<IResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);

    }

}
       

      


        

       

