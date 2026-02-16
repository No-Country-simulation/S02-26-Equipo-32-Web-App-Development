using Application.Dtos.Roles;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.Roles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleReadDto>()
                .ForMember(dest => dest.UserCount, opt => opt.Ignore()); // Se asigna manualmente en el servicio
        }
    }
}
