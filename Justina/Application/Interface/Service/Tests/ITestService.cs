using Application.Dtos.Tests;
using Application.Interface.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Service.Tests
{
    public interface ITestService
    {
        Task<IResult<TestResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<TestResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IResult<TestResponseDto>> CreateAsync(TestRequestDto request, CancellationToken cancellationToken = default);
        Task<IResult<TestResponseDto>> UpdateAsync(TestUpdateDto request, CancellationToken cancellationToken = default);
        Task<IResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
