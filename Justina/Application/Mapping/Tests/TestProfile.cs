using Application.Dtos.Tests;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.Tests
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            // Request -> Entity
            CreateMap<TestRequestDto, Test>();

            // Update -> Entity
            CreateMap<TestUpdateDto, Test>();

            // Entity -> Response
            CreateMap<Test, TestResponseDto>()
                .ForMember(dest => dest.DifficultyDescription,
                          opt => opt.MapFrom(src => src.Difficulty.Description));
        }
    }
}
