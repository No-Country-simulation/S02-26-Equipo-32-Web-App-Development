using Application.Dtos.Common;          // 👈 NUEVO
using Application.Dtos.Users;
using Application.Interface.Repository;
using Application.Interface.Service;
using Application.Interface.UnitOfWork;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Application.Service.Users
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ApiResponse<int>> RegisterAsync(RegisterUserDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user != null)
                {
                    return ApiResponse<int>.Error(400, "Ya existe un usuario con este email");
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var newUser = new User
                {
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserRoles = new List<UserRole>()
                };

                // Asignar rol por defecto según lo que eligió el usuario
                var defaultRole = await _roleRepository.GetByNameAsync("User");
                if (defaultRole != null)
                {
                    newUser.UserRoles.Add(new UserRole
                    {
                        User = newUser,
                        RoleId = defaultRole.Id,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = "System"
                    });
                }

                await _userRepository.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<int>.Created(newUser.Id, "Usuario registrado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginUserDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                    return ApiResponse<LoginResponseDto>.Error(401, "Credenciales inválidas");

                bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!passwordValid)
                    return ApiResponse<LoginResponseDto>.Error(401, "Credenciales inválidas");

                // Obtener roles del usuario
                var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
                var roles = userWithRoles?.UserRoles.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = GenerateToken(claims);

                var response = new LoginResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles
                };

                return ApiResponse<LoginResponseDto>.Success(response, "Login exitoso");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto, string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return ApiResponse<bool>.Error(404, "Usuario no encontrado");

                bool currentPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
                if (!currentPasswordValid)
                    return ApiResponse<bool>.Error(400, "Contraseña actual incorrecta");

                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return ApiResponse<bool>.Error(400, "Las contraseñas no coinciden");

                if (dto.NewPassword.Length < 6)
                    return ApiResponse<bool>.Error(400, "La contraseña debe tener al menos 6 caracteres");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        private string GenerateToken(List<Claim> claims)
        {
            var tokenKey = _configuration["AppSettings:Token"]
                ?? throw new InvalidOperationException("Token no configurado");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}