using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class EmailServiceRepo : IEmailServiceRepo
    {
        private readonly SmtpClient _smtpClient;

        public EmailServiceRepo()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harshabc10@gmail.com", "30thedoctor"),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var mailMessage = new MailMessage("harshabc10@gmail.com", recipientEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
