using Application.Dtos.UserRole;
using Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.UsersRoles
{
    public class UserRoleMappingProfile : Profile
    {
        public UserRoleMappingProfile()
        {
            // Entity -> DTO
            CreateMap<UserRole, UserRoleDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FirstName : null))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null));

            // CreateDTO -> Entity
            CreateMap<CreateUserRoleDto, UserRole>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // UpdateDTO -> Entity
            CreateMap<UpdateUserRoleDto, UserRole>()
                .ForMember(dest => dest.AssignedAt, opt => opt.Ignore()) // No actualizamos la fecha de asignación
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }
    }
}
