using Application.Dtos.Auth;
using Application.Dtos.Common;
using Application.Interface.Repository;
using Application.Interface.Service;
using Application.Interface.UnitOfWork;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public class PasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository; // 👈 NUEVO REPOSITORIO
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PasswordResetService(
            IUserRepository userRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository, // 👈 AGREGAR
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository; // 👈 ASIGNAR
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    return ApiResponse<bool>.Success(true, "Si el email existe, recibirás instrucciones");
                }

                // Generar token seguro
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                    .Replace("/", "_").Replace("+", "-");

                var resetToken = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiryDate = DateTime.UtcNow.AddHours(1),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                // ✅ GUARDAR TOKEN EN BD
                await _passwordResetTokenRepository.AddAsync(resetToken);
                await _unitOfWork.SaveChangesAsync();

                // Construir link de reset (frontend)
                var resetLink = $"{_configuration["AppUrl"]}/reset-password?token={token}";

                // Enviar email
                await _emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    resetLink
                );

                return ApiResponse<bool>.Success(true, "Si el email existe, recibirás instrucciones");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error(500, $"Error al procesar solicitud: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                // ✅ 1. BUSCAR TOKEN VÁLIDO
                var resetToken = await _passwordResetTokenRepository.GetValidTokenAsync(dto.Token);

                if (resetToken == null || resetToken.ExpiryDate < DateTime.UtcNow || resetToken.IsUsed)
                {
                    return ApiResponse<bool>.Error(400, "Token inválido o expirado");
                }

                // ✅ 2. OBTENER USUARIO
                var user = await _userRepository.GetByIdAsync(resetToken.UserId);
                if (user == null)
                {
                    return ApiResponse<bool>.Error(404, "Usuario no encontrado");
                }

                // ✅ 3. ACTUALIZAR CONTRASEÑA (GUARDADO REAL)
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                _userRepository.Update(user);

                // ✅ 4. MARCAR TOKEN COMO USADO
                resetToken.IsUsed = true;
                _passwordResetTokenRepository.Update(resetToken);

                // ✅ 5. GUARDAR CAMBIOS
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Contraseña actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error(500, $"Error al restablecer contraseña: {ex.Message}");
            }
        }
    }
}