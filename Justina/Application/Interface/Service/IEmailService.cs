using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Service
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
        Task SendDeletionConfirmationEmailAsync(string toEmail, string userName, DateTime deletionDate);
        Task SendDeletionCancelledEmailAsync(string toEmail, string userName);
    }
}
