using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Users
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
