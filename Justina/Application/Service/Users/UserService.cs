using Application.Dtos.Common;
using Application.Dtos.Users;
using Application.Interface.Repository;
using Application.Interface.Service;
using Application.Interface.UnitOfWork;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;

        }

        #region ADMIN - Gestión de usuarios

        public async Task<ApiResponse<IEnumerable<UserReadDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllWithRolesAsync();
                var usersDto = _mapper.Map<IEnumerable<UserReadDto>>(users);

                return ApiResponse<IEnumerable<UserReadDto>>.Success(usersDto, "Usuarios obtenidos correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return ApiResponse<IEnumerable<UserReadDto>>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserReadDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(id);
                if (user == null)
                    return ApiResponse<UserReadDto>.Error(404, "Usuario no encontrado");

                var userDto = _mapper.Map<UserReadDto>(user);
                return ApiResponse<UserReadDto>.Success(userDto, "Usuario obtenido correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
                return ApiResponse<UserReadDto>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserReadDto>> CreateUserAsync(UserCreateDto createDto)
        {
            try
            {
                // Validar email único
                var existingUser = await _userRepository.GetByEmailAsync(createDto.Email);
                if (existingUser != null)
                    return ApiResponse<UserReadDto>.Error(400, "El email ya está registrado");

                // Validar que los roles existan
                var roles = await _roleRepository.GetAllAsync();
                var validRoles = roles.Select(r => r.Id).ToList();
                var invalidRoles = createDto.RoleIds.Except(validRoles).ToList();

                if (invalidRoles.Any())
                    return ApiResponse<UserReadDto>.Error(400, $"Roles inválidos: {string.Join(", ", invalidRoles)}");

                // Crear usuario
                var user = new User
                {
                    Email = createDto.Email,
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    PhoneNumber = createDto.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedBy = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserRoles = new List<UserRole>()
                };

                // Asignar roles
                foreach (var roleId in createDto.RoleIds)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = "Admin"
                    });
                }

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserReadDto>(user);
                return ApiResponse<UserReadDto>.Success(userDto, "Usuario creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return ApiResponse<UserReadDto>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserReadDto>> UpdateUserAsync(int id, UserAdminUpdateDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(id);
                if (user == null)
                    return ApiResponse<UserReadDto>.Error(404, "Usuario no encontrado");

                // Si cambia email, verificar que no exista
                if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
                {
                    var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
                    if (existingUser != null)
                        return ApiResponse<UserReadDto>.Error(400, "El email ya está registrado por otro usuario");

                    user.Email = updateDto.Email;
                    user.EmailConfirmed = false; // Requiere reconfirmación
                }

                // Actualizar datos básicos
                user.FirstName = updateDto.FirstName;
                user.LastName = updateDto.LastName;
                user.PhoneNumber = updateDto.PhoneNumber;
                user.IsActive = updateDto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = "Admin";

                // Actualizar roles
                user.UserRoles.Clear();
                foreach (var roleId in updateDto.RoleIds)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = "Admin"
                    });
                }

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserReadDto>(user);
                return ApiResponse<UserReadDto>.Success(userDto, "Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
                return ApiResponse<UserReadDto>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> EnableUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return ApiResponse<bool>.Error(404, "Usuario no encontrado");

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = "Admin";

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Usuario habilitado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al habilitar usuario {UserId}", id);
                return ApiResponse<bool>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DisableUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return ApiResponse<bool>.Error(404, "Usuario no encontrado");

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = "Admin";

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Usuario deshabilitado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al deshabilitar usuario {UserId}", id);
                return ApiResponse<bool>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        #endregion

        #region USUARIO - Perfil propio

        public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(userId);
                if (user == null)
                    return ApiResponse<UserProfileDto>.Error(404, "Usuario no encontrado");

                var profileDto = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };

                return ApiResponse<UserProfileDto>.Success(profileDto, "Perfil obtenido correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil del usuario {UserId}", userId);
                return ApiResponse<UserProfileDto>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(int userId, UserProfileUpdateDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdWithRolesAsync(userId);
                if (user == null)
                    return ApiResponse<UserProfileDto>.Error(404, "Usuario no encontrado");

                // Actualizar solo campos permitidos
                user.FirstName = updateDto.FirstName;
                user.LastName = updateDto.LastName;
                user.PhoneNumber = updateDto.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = user.Email;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Obtener perfil actualizado
                var profileDto = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };

                return ApiResponse<UserProfileDto>.Success(profileDto, "Perfil actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil del usuario {UserId}", userId);
                return ApiResponse<UserProfileDto>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RequestAccountDeletionAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<bool>.Error(404, "Usuario no encontrado");

                // Marcar solicitud de eliminación
                user.DeletionRequested = true;
                user.DeletionRequestedAt = DateTime.UtcNow;
                user.ScheduledDeletionDate = DateTime.UtcNow.AddDays(30);
                user.IsActive = false; // Deshabilitar acceso inmediato
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = user.Email;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Enviar email de confirmación
                await _emailService.SendDeletionConfirmationEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    user.ScheduledDeletionDate.Value
                );

                return ApiResponse<bool>.Success(true,
                    "Solicitud de eliminación recibida. Tienes 30 días para cancelarla.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al solicitar eliminación de cuenta");
                return ApiResponse<bool>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CancelDeletionRequestAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<bool>.Error(404, "Usuario no encontrado");

                if (!user.DeletionRequested)
                    return ApiResponse<bool>.Error(400, "No hay solicitud de eliminación activa");

                // Cancelar solicitud
                user.DeletionRequested = false;
                user.DeletionRequestedAt = null;
                user.ScheduledDeletionDate = null;
                user.IsActive = true; // Reactivar cuenta
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = user.Email;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Enviar email de confirmación
                await _emailService.SendDeletionCancelledEmailAsync(user.Email, $"{user.FirstName} {user.LastName}");

                return ApiResponse<bool>.Success(true, "Solicitud de eliminación cancelada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar solicitud de eliminación");
                return ApiResponse<bool>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DeletionRequestDto>>> GetDeletionRequestsAsync()
        {
            try
            {
                var users = await _userRepository.GetDeletionRequestsAsync();

                var requests = users.Select(u => new DeletionRequestDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = $"{u.FirstName} {u.LastName}",
                    RequestedAt = u.DeletionRequestedAt!.Value,
                    ScheduledDeletion = u.ScheduledDeletionDate!.Value
                });

                return ApiResponse<IEnumerable<DeletionRequestDto>>.Success(requests,
                    "Solicitudes de eliminación obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes de eliminación");
                return ApiResponse<IEnumerable<DeletionRequestDto>>.Error(500, $"Error interno: {ex.Message}");
            }
        }

        #endregion
    }
}
