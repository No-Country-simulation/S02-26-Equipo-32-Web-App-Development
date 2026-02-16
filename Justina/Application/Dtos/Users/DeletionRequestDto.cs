using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Users
{
    public class DeletionRequestDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime ScheduledDeletion { get; set; }
        public int DaysRemaining => (ScheduledDeletion - DateTime.UtcNow).Days;
    }
}
