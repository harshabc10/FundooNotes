using BuisinessLayer.service.Iservice;
using Google.Apis.Gmail.v1;
using Microsoft.Extensions.Logging;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
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
        private readonly ILogger <CollaboratorService> _logger;
        

        public CollaboratorService(ICollaboratorRepository collaboratorRepository, ILogger<CollaboratorService> logger)
        {
            _collaboratorRepository = collaboratorRepository ?? throw new ArgumentNullException(nameof(collaboratorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CollaboratorRequest> AddCollaborator(string userId, CollaboratorRequest collaborator)
        {
            // Validate input (optional)

            // Add collaborator to the repository
            var addedCollaborator = await _collaboratorRepository.AddCollaborator(userId,collaborator);

            // Send invitation email to the collaborator
            /*await SendCollaboratorInvitationEmail(collaborator.CollaboratorEmail);*/

            return addedCollaborator;
        }

        public async Task<bool> DeleteCollaboratorById(string userId, int collaboratorId)
        {
            // Delete collaborator from the repository
            var isDeleted = await _collaboratorRepository.DeleteCollaboratorById(userId,collaboratorId);

            return isDeleted;
        }

        public async Task<Collaborator> GetCollaboratorById(string userId, int collaboratorId)
        {
            // Get collaborator from the repository
            var collaborator = await _collaboratorRepository.GetCollaboratorById(userId,collaboratorId);

            return collaborator;
        }

        /*        private async Task SendCollaboratorInvitationEmail(string email)
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
                }*/
    }
}
