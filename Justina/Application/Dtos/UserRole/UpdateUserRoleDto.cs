using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.UserRole
{
    public record UpdateUserRoleDto
    {
        [Required(ErrorMessage = "El Id es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id debe ser mayor a 0")]
        public int Id { get; init; }

        [Required(ErrorMessage = "El UserId es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El UserId debe ser mayor a 0")]
        public int UserId { get; init; }

        [Required(ErrorMessage = "El RoleId es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El RoleId debe ser mayor a 0")]
        public int RoleId { get; init; }

        [Required(ErrorMessage = "El campo AssignedBy es obligatorio")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "AssignedBy debe tener entre 1 y 256 caracteres")]
        public string AssignedBy { get; init; } = string.Empty;
    }
}
