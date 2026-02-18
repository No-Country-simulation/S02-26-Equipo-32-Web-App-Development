using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.UserRole
{
  
    public record UserRoleDto
    {
        public int Id { get; init; }

        public int UserId { get; init; }

        public int RoleId { get; init; }

        public DateTime AssignedAt { get; init; }

        public string AssignedBy { get; init; } = string.Empty;

        // Propiedades de navegación opcionales para información adicional
        public string? UserName { get; init; }

        public string? RoleName { get; init; }
    }
}
