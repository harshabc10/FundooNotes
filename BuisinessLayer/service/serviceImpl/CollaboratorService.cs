using BuisinessLayer.service.Iservice;
using Google.Apis.Gmail.v1;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ICollaboratorRepository _collaboratorRepository;
        private readonly IEmailService _emailService; // Assume you have an email service

        public CollaboratorService(ICollaboratorRepository collaboratorRepository, IEmailService emailService)
        {
            _collaboratorRepository = collaboratorRepository;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator)
        {
            // Validate input (optional)

            // Add collaborator to the repository
            var addedCollaborator = await _collaboratorRepository.AddCollaboratorAsync(collaborator);

            // Send invitation email to the collaborator
            await SendCollaboratorInvitationEmail(collaborator.CollaboratorEmail);

            return addedCollaborator;
        }

        public async Task<bool> DeleteCollaboratorAsync(int collaboratorId)
        {
            // Delete collaborator from the repository
            var isDeleted = await _collaboratorRepository.DeleteCollaboratorAsync(collaboratorId);

            return isDeleted;
        }

        public async Task<Collaborator> GetCollaboratorAsync(int collaboratorId)
        {
            // Get collaborator from the repository
            var collaborator = await _collaboratorRepository.GetCollaboratorAsync(collaboratorId);

            return collaborator;
        }

        private async Task SendCollaboratorInvitationEmail(string email)
        {
            // Validate email (optional)

            // Send invitation email to the collaborator using the email service
            try
            {
                await _emailService.SendEmailAsync(email, "Invitation to collaborate", "You have been invited as a collaborator.");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error sending invitation email to collaborator.", ex);
            }
        }
    }
}
