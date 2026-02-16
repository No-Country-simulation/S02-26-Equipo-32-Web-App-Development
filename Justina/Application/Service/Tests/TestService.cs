using Application.Dtos.Tests;
using Application.Interface.Repository.Tests;
using Application.Interface.Result;
using Application.Interface.Service.Tests;
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

namespace Application.Service.Tests
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TestService> _logger;

        public TestService(
            ITestRepository testRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TestService> logger)
        {
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult<TestResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo Test con ID: {TestId}", id);

                var test = await _testRepository.GetByIdAsync(id, cancellationToken);

                if (test == null)
                {
                    _logger.LogWarning("Test con ID: {TestId} no encontrado", id);
                    return Result<TestResponseDto>.Failure($"Test con ID {id} no encontrado.");
                }

                var testDto = _mapper.Map<TestResponseDto>(test);
                _logger.LogInformation("Test con ID: {TestId} obtenido exitosamente", id);

                return Result<TestResponseDto>.Success(testDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Test con ID: {TestId}", id);
                return Result<TestResponseDto>.Failure($"Error al obtener el test: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<TestResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los Tests");

                var tests = await _testRepository.GetAllAsync(cancellationToken);
                var testsDto = _mapper.Map<IEnumerable<TestResponseDto>>(tests);

                _logger.LogInformation("Se obtuvieron {Count} Tests", testsDto.Count());

                return Result<IEnumerable<TestResponseDto>>.Success(testsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Tests");
                return Result<IEnumerable<TestResponseDto>>.Failure($"Error al obtener los tests: {ex.Message}");
            }
        }

        public async Task<IResult<TestResponseDto>> CreateAsync(TestRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creando nuevo Test: {TestName}", request.Name);

                var test = _mapper.Map<Test>(request);
                var createdTest = await _testRepository.AddAsync(test, cancellationToken);

                var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (saved <= 0)
                {
                    _logger.LogWarning("No se pudo guardar el Test en la base de datos");
                    return Result<TestResponseDto>.Failure("No se pudo crear el test.");
                }

                // Cargar la relación Difficulty para el DTO
                var testWithRelations = await _testRepository.GetByIdAsync(createdTest.Id, cancellationToken);
                var testDto = _mapper.Map<TestResponseDto>(testWithRelations);

                _logger.LogInformation("Test creado exitosamente con ID: {TestId}", createdTest.Id);

                return Result<TestResponseDto>.Success(testDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Test: {TestName}", request.Name);
                return Result<TestResponseDto>.Failure($"Error al crear el test: {ex.Message}");
            }
        }

        public async Task<IResult<TestResponseDto>> UpdateAsync(TestUpdateDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Actualizando Test con ID: {TestId}", request.Id);

                var test = _mapper.Map<Test>(request);
                var updatedTest = _testRepository.Update(test);

                if (updatedTest == null)
                {
                    _logger.LogWarning("Test con ID: {TestId} no encontrado para actualizar", request.Id);
                    return Result<TestResponseDto>.Failure($"Test con ID {request.Id} no encontrado.");
                }

                var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (saved <= 0)
                {
                    _logger.LogWarning("No se pudo actualizar el Test con ID: {TestId}", request.Id);
                    return Result<TestResponseDto>.Failure("No se pudo actualizar el test.");
                }

                // Cargar la relación Difficulty para el DTO
                var testWithRelations = await _testRepository.GetByIdAsync(request.Id, cancellationToken);
                var testDto = _mapper.Map<TestResponseDto>(testWithRelations);

                _logger.LogInformation("Test con ID: {TestId} actualizado exitosamente", request.Id);

                return Result<TestResponseDto>.Success(testDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar Test con ID: {TestId}", request.Id);
                return Result<TestResponseDto>.Failure($"Error al actualizar el test: {ex.Message}");
            }
        }

        public async Task<IResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Eliminando Test con ID: {TestId}", id);

                var deleted = _testRepository.Delete(id);

                if (!deleted)
                {
                    _logger.LogWarning("Test con ID: {TestId} no encontrado para eliminar", id);
                    return Result<bool>.Failure($"Test con ID {id} no encontrado.");
                }

                var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (saved <= 0)
                {
                    _logger.LogWarning("No se pudo eliminar el Test con ID: {TestId}", id);
                    return Result<bool>.Failure("No se pudo eliminar el test.");
                }

                _logger.LogInformation("Test con ID: {TestId} eliminado exitosamente", id);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar Test con ID: {TestId}", id);
                return Result<bool>.Failure($"Error al eliminar el test: {ex.Message}");
            }
        }
    }
}
