using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
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
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly DapperContext _context;

        public CollaboratorRepository(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator)
        {
            try
            {
                string sql = @"
                    INSERT INTO Collaborators (UserId, UserNoteId, CollaboratorEmail)
                    VALUES (@UserId, @UserNoteId, @CollaboratorEmail);
                    SELECT SCOPE_IDENTITY();";

                using (var connection = _context.CreateConnection())
                {
                    int id = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        collaborator.UserId,
                        collaborator.UserNoteId,
                        collaborator.CollaboratorEmail
                    });

                    collaborator.CollaboratorId = id;

                    // Send email to collaborator
                    await SendEmail(collaborator.CollaboratorEmail, "You have been added as a collaborator", "You have been added as a collaborator to a note.");
                    return collaborator;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error adding collaborator to the database.", ex);
            }
        }

        private async Task SendEmail(string toEmail, string subject, string body)
        {
            // Configure SMTP client for Outlook
            var smtpClient = new SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harshabc10@outlook.com", "30thedoctor"),
                EnableSsl = true,
            };

            // Create mail message
            var mailMessage = new MailMessage
            {
                From = new MailAddress("harshabc10@outlook.com", "Added As Collaborator"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false, // Change to true if sending HTML emails
            };
            mailMessage.To.Add(toEmail);

            // Send email
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<bool> DeleteCollaboratorAsync(int collaboratorId)
        {
            try
            {
                string sql = "DELETE FROM Collaborators WHERE CollaboratorId = @CollaboratorId";

                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, new { CollaboratorId = collaboratorId });
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error deleting collaborator from the database.", ex);
            }
        }

        public async Task<Collaborator> GetCollaboratorAsync(int collaboratorId)
        {
            try
            {
                string sql = "SELECT * FROM Collaborators WHERE CollaboratorId = @CollaboratorId";

                using (var connection = _context.CreateConnection())
                {
                    var collaborator = await connection.QueryFirstOrDefaultAsync<Collaborator>(sql, new { CollaboratorId = collaboratorId });
                    return collaborator;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error getting collaborator from the database.", ex);
            }
        }
    }
}
