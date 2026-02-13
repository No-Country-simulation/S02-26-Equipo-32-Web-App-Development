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
        private readonly IRoleRepository _roleRepository;    // 👈 AGREGAR
        private readonly IUnitOfWork _unitOfWork;           // 👈 AGREGAR
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;                   // 👈 AGREGAR (opcional)

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,                 // 👈 AGREGAR
            IUnitOfWork unitOfWork,                        // 👈 AGREGAR
            IConfiguration configuration,
            IMapper mapper)                                // 👈 AGREGAR (opcional)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;              // 👈 AGREGAR
            _unitOfWork = unitOfWork;                     // 👈 AGREGAR
            _configuration = configuration;
            _mapper = mapper;                             // 👈 AGREGAR (opcional)
        }

        public async Task<int> RegisterAsync(RegisterUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user != null)
            {
                throw new Exception("Ya existe un usuario con este email");
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

            // 👇 ASIGNAR ROL POR DEFECTO "PATIENT"
            var defaultRole = await _roleRepository.GetByNameAsync("Patient");
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
            await _unitOfWork.SaveChangesAsync();           // 👈 AGREGAR
            return newUser.Id;
        }

        public async Task<string> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid)
                throw new Exception("Contraseña incorrecta");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value ??
                throw new InvalidOperationException("Token no configurado")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto, string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            bool currentPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!currentPasswordValid)
                throw new Exception("Contraseña actual incorrecta");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new Exception("La nueva contraseña y la confirmación no coinciden");

            if (dto.NewPassword.Length < 6)
                throw new Exception("La nueva contraseña debe tener al menos 6 caracteres");

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.PasswordHash = newPasswordHash;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();           
        }
    }
}