using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BuisinessLayer.service.Iservice;

namespace BuisinessLayer.service.serviceImpl
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your-email@gmail.com", "your-email-password"),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var mailMessage = new MailMessage("your-email@gmail.com", recipientEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
