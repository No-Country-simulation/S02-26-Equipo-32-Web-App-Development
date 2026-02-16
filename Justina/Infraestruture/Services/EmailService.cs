using Application.Interface.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infraestruture.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
        {
            await SendEmailAsync(toEmail, "Recuperación de contraseña - Justina Simulator",
                $"""
                <h2>Hola {userName},</h2>
                <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                <p>Haz clic en el siguiente enlace para crear una nueva contraseña:</p>
                <p><a href='{resetLink}'>Restablecer contraseña</a></p>
                <p>Este enlace expirará en 1 hora.</p>
                <p>Si no solicitaste esto, ignora este mensaje.</p>
                <br>
                <p>Saludos,<br>Equipo Justina</p>
                """);
        }

        public async Task SendDeletionConfirmationEmailAsync(string toEmail, string userName, DateTime deletionDate)
        {
            await SendEmailAsync(toEmail, "Solicitud de eliminación de cuenta - Justina Simulator",
                $"""
                <h2>Hola {userName},</h2>
                <p>Hemos recibido tu solicitud de eliminación de cuenta.</p>
                <p>Tu cuenta será eliminada permanentemente el <strong>{deletionDate:dd/MM/yyyy}</strong>.</p>
                <p>Si no solicitaste esto, o cambiaste de opinión, puedes cancelar la solicitud iniciando sesión y usando la opción "Cancelar eliminación".</p>
                <p><a href='{_configuration["AppUrl"]}/profile/cancel-deletion'>Cancelar solicitud</a></p>
                <br>
                <p>Saludos,<br>Equipo Justina</p>
                """);
        }

        public async Task SendDeletionCancelledEmailAsync(string toEmail, string userName)
        {
            await SendEmailAsync(toEmail, "Solicitud de eliminación cancelada - Justina Simulator",
                $"""
                <h2>Hola {userName},</h2>
                <p>Tu solicitud de eliminación de cuenta ha sido cancelada exitosamente.</p>
                <p>Tu cuenta sigue activa y puedes seguir utilizando todos los servicios.</p>
                <br>
                <p>Saludos,<br>Equipo Justina</p>
                """);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var appPassword = _configuration["EmailSettings:AppPassword"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}