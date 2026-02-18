using Application.Dtos.UserRole;
using Application.Interface.Repository.UsersRoles;
using Application.Interface.Result;
using Application.Interface.Service.UsersRoles;
using Application.Interface.UnitOfWor;
using Application.Service.Result;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.UsersRoles
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(
            IUserRoleRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserRoleService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult<IEnumerable<UserRoleDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los UserRoles");

                var userRoles = await _repository.GetAllAsync(cancellationToken);
                var userRoleDtos = _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);

                _logger.LogInformation("Se obtuvieron {Count} UserRoles exitosamente", userRoleDtos.Count());

                return Result<IEnumerable<UserRoleDto>>.Success(userRoleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los UserRoles");
                return Result<IEnumerable<UserRoleDto>>.Failure($"Error al obtener los UserRoles: {ex.Message}");
            }
        }

        public async Task<IResult<UserRoleDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo UserRole con Id: {Id}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Id inválido proporcionado: {Id}", id);
                    return Result<UserRoleDto>.Failure("El Id debe ser mayor a 0");
                }

                var userRole = await _repository.GetByIdAsync(id, cancellationToken);

                if (userRole == null)
                {
                    _logger.LogWarning("UserRole con Id {Id} no encontrado", id);
                    return Result<UserRoleDto>.Failure($"UserRole con Id {id} no encontrado");
                }

                var userRoleDto = _mapper.Map<UserRoleDto>(userRole);

                _logger.LogInformation("UserRole con Id {Id} obtenido exitosamente", id);

                return Result<UserRoleDto>.Success(userRoleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener UserRole con Id: {Id}", id);
                return Result<UserRoleDto>.Failure($"Error al obtener el UserRole: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<UserRoleDto>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo roles del usuario con Id: {UserId}", userId);

                if (userId <= 0)
                {
                    _logger.LogWarning("UserId inválido proporcionado: {UserId}", userId);
                    return Result<IEnumerable<UserRoleDto>>.Failure("El UserId debe ser mayor a 0");
                }

                var userRoles = await _repository.GetByUserIdAsync(userId, cancellationToken);
                var userRoleDtos = _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);

                _logger.LogInformation("Se obtuvieron {Count} roles para el usuario {UserId}", userRoleDtos.Count(), userId);

                return Result<IEnumerable<UserRoleDto>>.Success(userRoleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles del usuario con Id: {UserId}", userId);
                return Result<IEnumerable<UserRoleDto>>.Failure($"Error al obtener los roles del usuario: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<UserRoleDto>>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios con el rol Id: {RoleId}", roleId);

                if (roleId <= 0)
                {
                    _logger.LogWarning("RoleId inválido proporcionado: {RoleId}", roleId);
                    return Result<IEnumerable<UserRoleDto>>.Failure("El RoleId debe ser mayor a 0");
                }

                var userRoles = await _repository.GetByRoleIdAsync(roleId, cancellationToken);
                var userRoleDtos = _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);

                _logger.LogInformation("Se obtuvieron {Count} usuarios con el rol {RoleId}", userRoleDtos.Count(), roleId);

                return Result<IEnumerable<UserRoleDto>>.Success(userRoleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios con el rol Id: {RoleId}", roleId);
                return Result<IEnumerable<UserRoleDto>>.Failure($"Error al obtener usuarios con el rol: {ex.Message}");
            }
        }
        public async Task<IResult<IEnumerable<UserRoleDto>>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios con el rol de nombre: {RoleName}", roleName);

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogWarning("Nombre de rol vacío o nulo proporcionado");
                    return Result<IEnumerable<UserRoleDto>>.Failure("El nombre del rol no puede estar vacío");
                }

                var userRoles = await _repository.GetByRoleNameAsync(roleName, cancellationToken);
                var userRoleDtos = _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);

                _logger.LogInformation("Se obtuvieron {Count} usuarios con el rol '{RoleName}'", userRoleDtos.Count(), roleName);

                return Result<IEnumerable<UserRoleDto>>.Success(userRoleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios con el rol de nombre: {RoleName}", roleName);
                return Result<IEnumerable<UserRoleDto>>.Failure($"Error al obtener usuarios con el rol: {ex.Message}");
            }
        }


        public async Task<IResult<UserRoleDto>> CreateAsync(CreateUserRoleDto createDto, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creando nuevo UserRole para UserId: {UserId} y RoleId: {RoleId}",
                    createDto.UserId, createDto.RoleId);

                // Verificar si ya existe la asignación
                var exists = await _repository.ExistsAsync(createDto.UserId, createDto.RoleId, cancellationToken);
                if (exists)
                {
                    _logger.LogWarning("Ya existe una asignación de rol para UserId: {UserId} y RoleId: {RoleId}",
                        createDto.UserId, createDto.RoleId);
                    return Result<UserRoleDto>.Failure("Ya existe una asignación de este rol para el usuario");
                }

                var userRole = _mapper.Map<UserRole>(createDto);

                await _repository.AddAsync(userRole, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var createdUserRole = await _repository.GetByIdAsync(userRole.Id, cancellationToken);
                var userRoleDto = _mapper.Map<UserRoleDto>(createdUserRole);

                _logger.LogInformation("UserRole creado exitosamente con Id: {Id}", userRole.Id);

                return Result<UserRoleDto>.Success(userRoleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear UserRole para UserId: {UserId} y RoleId: {RoleId}",
                    createDto.UserId, createDto.RoleId);
                return Result<UserRoleDto>.Failure($"Error al crear el UserRole: {ex.Message}");
            }
        }

        public async Task<IResult<UserRoleDto>> UpdateAsync(UpdateUserRoleDto updateDto, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Actualizando UserRole con Id: {Id}", updateDto.Id);

                var existingUserRole = await _repository.GetByIdAsync(updateDto.Id, cancellationToken);

                if (existingUserRole == null)
                {
                    _logger.LogWarning("UserRole con Id {Id} no encontrado para actualización", updateDto.Id);
                    return Result<UserRoleDto>.Failure($"UserRole con Id {updateDto.Id} no encontrado");
                }

                // Verificar si la nueva combinación de UserId y RoleId ya existe en otro registro
                if (existingUserRole.UserId != updateDto.UserId || existingUserRole.RoleId != updateDto.RoleId)
                {
                    var exists = await _repository.ExistsAsync(updateDto.UserId, updateDto.RoleId, cancellationToken);
                    if (exists)
                    {
                        _logger.LogWarning("Ya existe otra asignación para UserId: {UserId} y RoleId: {RoleId}",
                            updateDto.UserId, updateDto.RoleId);
                        return Result<UserRoleDto>.Failure("Ya existe una asignación de este rol para el usuario");
                    }
                }

                _mapper.Map(updateDto, existingUserRole);

                _repository.Update(existingUserRole);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var updatedUserRole = await _repository.GetByIdAsync(updateDto.Id, cancellationToken);
                var userRoleDto = _mapper.Map<UserRoleDto>(updatedUserRole);

                _logger.LogInformation("UserRole con Id {Id} actualizado exitosamente", updateDto.Id);

                return Result<UserRoleDto>.Success(userRoleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar UserRole con Id: {Id}", updateDto.Id);
                return Result<UserRoleDto>.Failure($"Error al actualizar el UserRole: {ex.Message}");
            }
        }

        public async Task<IResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Eliminando UserRole con Id: {Id}", id);

                if (id <= 0)
                {
                    _logger.LogWarning("Id inválido proporcionado para eliminación: {Id}", id);
                    return Result<bool>.Failure("El Id debe ser mayor a 0");
                }

                var userRole = await _repository.GetByIdAsync(id, cancellationToken);

                if (userRole == null)
                {
                    _logger.LogWarning("UserRole con Id {Id} no encontrado para eliminación", id);
                    return Result<bool>.Failure($"UserRole con Id {id} no encontrado");
                }

                _repository.Delete(userRole);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("UserRole con Id {Id} eliminado exitosamente", id);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar UserRole con Id: {Id}", id);
                return Result<bool>.Failure($"Error al eliminar el UserRole: {ex.Message}");
            }
        }
    }
}
