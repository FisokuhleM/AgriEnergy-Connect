using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace AgriEnergyConnect.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = "fisosingulvrity.mkhize@gmail.com";
            var appPassword = "fnvalcfmkwtwntfp";

            MailMessage message = new()
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(email));

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587)) // Using port 587 for TLS/STARTTLS
            {
                smtpClient.Credentials = new NetworkCredential(fromEmail, appPassword);
                smtpClient.EnableSsl = true; // This ensures that TLS/STARTTLS is used


                await smtpClient.SendMailAsync(message);

            }
        }
    }
}
