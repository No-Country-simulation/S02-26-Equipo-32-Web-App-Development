using Application.Dtos.Roles;
using Application.Interface.Repository;
using Application.Interface.Result;
using Application.Interface.Service;
using Application.Interface.UnitOfWork;
using Application.Service.Result;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD Básico

        public async Task<IResult<RoleReadDto>> CreateRoleAsync(RoleCreateDto createDto)
        {
            try
            {
                // 1. Validar si ya existe un rol con ese nombre
                var existingRole = await _roleRepository.GetByNameAsync(createDto.Name);
                if (existingRole != null)
                    return Result<RoleReadDto>.Failure($"Ya existe un rol con el nombre '{createDto.Name}'");

                // 2. Crear nueva entidad Role
                var role = new Role
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system" // TODO: Obtener del usuario autenticado
                };

                // 3. Guardar en base de datos
                await _roleRepository.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();

                // 4. Mapear a DTO y retornar
                var roleDto = _mapper.Map<RoleReadDto>(role);
                roleDto.UserCount = 0;

                _logger.LogInformation("Rol creado exitosamente: {RoleName} (ID: {RoleId})", role.Name, role.Id);
                return Result<RoleReadDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol: {RoleName}", createDto.Name);
                return Result<RoleReadDto>.Failure($"Error al crear rol: {ex.Message}");
            }
        }

        public async Task<IResult<RoleReadDto>> GetRoleByIdAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                    return Result<RoleReadDto>.Failure("Rol no encontrado");

                var roleDto = _mapper.Map<RoleReadDto>(role);
                roleDto.UserCount = await _roleRepository.GetUserCountByRoleIdAsync(id);

                return Result<RoleReadDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol por ID: {RoleId}", id);
                return Result<RoleReadDto>.Failure($"Error al obtener rol: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<RoleReadDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllActiveAsync();
                var rolesDto = _mapper.Map<IEnumerable<RoleReadDto>>(roles);

                foreach (var roleDto in rolesDto)
                {
                    roleDto.UserCount = await _roleRepository.GetUserCountByRoleIdAsync(roleDto.Id);
                }

                return Result<IEnumerable<RoleReadDto>>.Success(rolesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                return Result<IEnumerable<RoleReadDto>>.Failure($"Error al obtener roles: {ex.Message}");
            }
        }

        public async Task<IResult<RoleReadDto>> UpdateRoleAsync(int id, RoleUpdateDto updateDto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                    return Result<RoleReadDto>.Failure("Rol no encontrado");

                // Validar si el nombre ya existe en otro rol
                var existingRole = await _roleRepository.GetByNameAsync(updateDto.Name);
                if (existingRole != null && existingRole.Id != id)
                    return Result<RoleReadDto>.Failure($"Ya existe otro rol con el nombre '{updateDto.Name}'");

                // Actualizar propiedades
                role.Name = updateDto.Name;
                role.Description = updateDto.Description;
                role.IsActive = updateDto.IsActive;
                role.UpdatedAt = DateTime.UtcNow;
                role.UpdatedBy = "system"; // TODO: Obtener del usuario autenticado

                _roleRepository.Update(role);
                await _unitOfWork.SaveChangesAsync();

                var roleDto = _mapper.Map<RoleReadDto>(role);
                roleDto.UserCount = await _roleRepository.GetUserCountByRoleIdAsync(id);

                _logger.LogInformation("Rol actualizado exitosamente: {RoleName} (ID: {RoleId})", role.Name, role.Id);
                return Result<RoleReadDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol ID: {RoleId}", id);
                return Result<RoleReadDto>.Failure($"Error al actualizar rol: {ex.Message}");
            }
        }

        public async Task<IResult<bool>> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                    return Result<bool>.Failure("Rol no encontrado");

                // No permitir eliminar roles por defecto del sistema
                if (role.Name == "Admin" || role.Name == "Doctor" || role.Name == "Patient")
                    return Result<bool>.Failure("No se puede eliminar un rol por defecto del sistema");

                var userCount = await _roleRepository.GetUserCountByRoleIdAsync(id);

                if (userCount > 0)
                {
                    // Soft delete si tiene usuarios asignados
                    role.IsActive = false;
                    role.UpdatedAt = DateTime.UtcNow;
                    role.UpdatedBy = "system";
                    _roleRepository.Update(role);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Rol desactivado (tiene {UserCount} usuarios): {RoleName}", userCount, role.Name);
                    return Result<bool>.Success(true);
                }
                else
                {
                    // Hard delete si no tiene usuarios
                    _roleRepository.Delete(role);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Rol eliminado permanentemente: {RoleName}", role.Name);
                    return Result<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol ID: {RoleId}", id);
                return Result<bool>.Failure($"Error al eliminar rol: {ex.Message}");
            }
        }

        #endregion

        #region Asignación de Roles

        public async Task<IResult<bool>> AssignRoleToUserAsync(AssignRoleDto assignDto)
        {
            try
            {
                // 1. Verificar que el usuario existe
                var user = await _userRepository.GetByIdWithRolesAsync(assignDto.UserId);
                if (user == null)
                    return Result<bool>.Failure("Usuario no encontrado");

                // 2. Verificar que el rol existe y está activo
                var role = await _roleRepository.GetByIdAsync(assignDto.RoleId);
                if (role == null || !role.IsActive)
                    return Result<bool>.Failure("Rol no encontrado o inactivo");

                // 3. Verificar que el usuario no tenga ya ese rol
                if (user.UserRoles.Any(ur => ur.RoleId == assignDto.RoleId))
                    return Result<bool>.Failure("El usuario ya tiene este rol asignado");

                // 4. Asignar el rol
                user.UserRoles.Add(new UserRole
                {
                    UserId = assignDto.UserId,
                    RoleId = assignDto.RoleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "system" // TODO: Obtener del usuario autenticado
                });

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Rol {RoleName} asignado al usuario {UserId}", role.Name, user.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar rol {RoleId} al usuario {UserId}", assignDto.RoleId, assignDto.UserId);
                return Result<bool>.Failure($"Error al asignar rol: {ex.Message}");
            }
        }

        public async Task<IResult<bool>> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                // 1. Verificar que el usuario existe
                var user = await _userRepository.GetByIdWithRolesAsync(userId);
                if (user == null)
                    return Result<bool>.Failure("Usuario no encontrado");

                // 2. Verificar que el usuario tiene ese rol
                var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
                if (userRole == null)
                    return Result<bool>.Failure("El usuario no tiene este rol asignado");

                // 3. Validaciones de negocio
                var role = await _roleRepository.GetByIdAsync(roleId);

                // No permitir quitar el rol Admin del último administrador
                if (role?.Name == "Admin")
                {
                    var adminCount = await _roleRepository.GetUserCountByRoleIdAsync(roleId);
                    if (adminCount <= 1)
                        return Result<bool>.Failure("No se puede remover el rol Admin del último administrador");
                }

                // 4. Remover el rol
                user.UserRoles.Remove(userRole);
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Rol {RoleName} removido del usuario {UserId}", role?.Name, user.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover rol {RoleId} del usuario {UserId}", roleId, userId);
                return Result<bool>.Failure($"Error al remover rol: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<RoleReadDto>>> GetRolesByUserIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return Result<IEnumerable<RoleReadDto>>.Failure("Usuario no encontrado");

                var roles = await _roleRepository.GetRolesByUserIdAsync(userId);
                var rolesDto = _mapper.Map<IEnumerable<RoleReadDto>>(roles);

                return Result<IEnumerable<RoleReadDto>>.Success(rolesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles del usuario {UserId}", userId);
                return Result<IEnumerable<RoleReadDto>>.Failure($"Error al obtener roles: {ex.Message}");
            }
        }

        #endregion

        #region Utilitarios

        public async Task<IResult<bool>> RoleExistsAsync(string roleName)
        {
            try
            {
                var role = await _roleRepository.GetByNameAsync(roleName);
                return Result<bool>.Success(role != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si existe el rol: {RoleName}", roleName);
                return Result<bool>.Failure($"Error al verificar rol: {ex.Message}");
            }
        }

        public async Task<IResult<int>> GetUserCountByRoleIdAsync(int roleId)
        {
            try
            {
                var count = await _roleRepository.GetUserCountByRoleIdAsync(roleId);
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de usuarios para el rol {RoleId}", roleId);
                return Result<int>.Failure($"Error al obtener conteo: {ex.Message}");
            }
        }

        #endregion
    }



}
