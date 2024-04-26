using Dapper;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using NLog;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger _logger;

        public CollaboratorRepository(DapperContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CollaboratorRequest> AddCollaborator(string userId, CollaboratorRequest collaborator)
        {
            try
            {
                // Check if the UserNoteId and UserId exist
                string checkExistenceSql = @"
            SELECT COUNT(*) 
            FROM UserNotes 
            WHERE Id = @UserNoteId AND UserId = @UserId;";

                using (var connection = _context.CreateConnection())
                {
                    int noteExists = await connection.ExecuteScalarAsync<int>(checkExistenceSql, new
                    {
                        UserId = userId,
                        collaborator.UserNoteId
                    });

                    if (noteExists == 0)
                    {
                        throw new Exception("UserNoteId or UserId not found.");
                    }

                    string insertSql = @"
            INSERT INTO Collaborators (UserId, UserNoteId, CollaboratorEmail)
            VALUES (@UserId, @UserNoteId, @CollaboratorEmail);
            SELECT SCOPE_IDENTITY();";

                    int collaboratorId = await connection.ExecuteScalarAsync<int>(insertSql, new
                    {
                        UserId = userId, // Use the provided userId parameter
                        collaborator.UserNoteId,
                        collaborator.CollaboratorEmail
                    });

                    // Send email to collaborator
                    await SendEmail(collaborator.CollaboratorEmail, "You have been added as a collaborator", "You have been added as a collaborator to a note.");
                    _logger.Info("Collaborator added successfully.");

                    // Return the collaborator with the updated CollaboratorId
                    //collaborator.CollaboratorId = collaboratorId;
                    return collaborator;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding collaborator to the database.");
                throw new Exception("Error adding collaborator to the database.", ex);
            }
        }




        private async Task SendEmail(string toEmail, string subject, string body)
        {
            try
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

                _logger.Info("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending email.");
                throw new Exception("Error sending email.", ex);
            }
        }

        public async Task<bool> DeleteCollaboratorById(string userId, int collaboratorId)
        {
            try
            {
                string sql = "DELETE FROM Collaborators WHERE UserId = @UserId AND CollaboratorId = @CollaboratorId";

                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, new { CollaboratorId = collaboratorId , UserId = userId });
                    _logger.Info("Deleted Collaborators Successfully");
                    return affectedRows > 0;

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting collaborator from the database.");
                throw new Exception("Error deleting collaborator from the database.", ex);
            }
        }


        public async Task<Collaborator> GetCollaboratorById(string userId, int collaboratorId)
        {
            try
            {
                string sql = "SELECT * FROM Collaborators WHERE  CollaboratorId = @CollaboratorId AND UserId = @UserId";

                using (var connection = _context.CreateConnection())
                {
                    var collaborator = await connection.QueryFirstOrDefaultAsync<Collaborator>(sql, new { CollaboratorId = collaboratorId, UserId = userId });
                    _logger.Info("Getting Collaborators Successfully");
                    return collaborator;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting collaborator from the database.");
                throw new Exception("Error getting collaborator from the database.", ex);
            }
        }
    }
}
