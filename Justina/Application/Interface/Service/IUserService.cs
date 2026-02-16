using Application.Dtos.Common;
using Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Service
{
    public interface IUserService
    {
        // PARA ADMIN
        Task<ApiResponse<IEnumerable<UserReadDto>>> GetAllUsersAsync();
        Task<ApiResponse<UserReadDto>> GetUserByIdAsync(int id);
        Task<ApiResponse<UserReadDto>> CreateUserAsync(UserCreateDto createDto);
        Task<ApiResponse<UserReadDto>> UpdateUserAsync(int id, UserAdminUpdateDto updateDto);
        Task<ApiResponse<bool>> EnableUserAsync(int id);
        Task<ApiResponse<bool>> DisableUserAsync(int id);

        // PARA USUARIO AUTENTICADO
        Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId);
        Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(int userId, UserProfileUpdateDto updateDto);

        Task<ApiResponse<bool>> RequestAccountDeletionAsync(int userId);      
        Task<ApiResponse<bool>> CancelDeletionRequestAsync(int userId);       
    }
}
