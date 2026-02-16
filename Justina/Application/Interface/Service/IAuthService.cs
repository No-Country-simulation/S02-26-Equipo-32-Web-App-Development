using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Common;
using Application.Dtos.Users;

namespace Application.Interface.Service
{
    public interface IAuthService
    {
        Task<ApiResponse<int>> RegisterAsync(RegisterUserDto dto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginUserDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto, string email);
    }
}
