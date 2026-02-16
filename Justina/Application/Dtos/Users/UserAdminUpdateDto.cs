using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Users
{
    public class UserAdminUpdateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Formato de email incorrecto")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public List<int> RoleIds { get; set; } = new();
    }
}
