using Application.Dtos.Users;
using Application.Interface.Repository;
using Application.Interface.Service;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                EmailConfirmed = true, //para que no moleste en desarrollo. En producción, esto debería ser false y requerir confirmación por email.
                CreatedBy = "System"
            };

            await _userRepository.AddAsync(newUser);
            return newUser.Id;
        }
    }
}
