using BuisinessLayer.service.Iservice;
using Google.Apis.Gmail.v1;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserNoteService : IUserNoteService
    {
        private readonly IUserNoteRepository _noteRepository;
        private readonly ILabelRepository _labelRepository;

        public UserNoteService(IUserNoteRepository noteRepository, ILabelRepository labelRepository)
        {
            _noteRepository = noteRepository;
            _labelRepository = labelRepository;
        }

        public async Task<UserNoteRequest> AddUserNote(UserNoteRequest note)
        {
            // Validate input
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note), "User note object cannot be null.");
            }

            // Additional validation logic can be added here

            // Map UserNote to data model (if needed)
            // For simplicity, assuming UserNote and data model are the same
            var userNoteToAdd = note;

            try
            {
                // Call repository method to add user note to the database
                var addedNote = await _noteRepository.AddUserNote(userNoteToAdd);
                return addedNote;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error adding user note to the database.", ex);
            }
        }


        public async Task<bool> DeleteUserNote(int id)
        {
            try
            {
                // Check if the user note with the specified ID exists
                var existingNote = await _noteRepository.GetUserNoteById(id);
                if (existingNote == null)
                {
                    // User note with the specified ID not found, return false
                    return false;
                }

                // Call the repository method to delete the user note
                bool isDeleted = await _noteRepository.DeleteUserNote(id);
                return isDeleted;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error deleting user note.", ex);
            }
        }

        public async Task<UserNote> UpdateUserNote(UserNote note)
        {
            // Validate input
            if (note == null || note.Id <= 0)
            {
                throw new ArgumentException("Invalid user note data.");
            }

            // Check if the user note with the specified ID exists
            var existingNote = await _noteRepository.GetUserNoteById(note.Id);
            if (existingNote == null)
            {
                throw new KeyNotFoundException("User note not found.");
            }

            // Update the properties of the existing note based on the provided note object
            existingNote.Title = note.Title;
            existingNote.Description = note.Description;
            existingNote.Color = note.Color;
            existingNote.ImagePaths = note.ImagePaths;
            existingNote.Reminder = note.Reminder;
            existingNote.IsArchive = note.IsArchive;
            existingNote.IsPinned = note.IsPinned;
            existingNote.IsTrash = note.IsTrash;
            //existingNote.CollaboratorIds = note.CollaboratorIds;

            // Call the repository method to update the user note in the database
            var updatedNote = await _noteRepository.UpdateUserNote(existingNote);

            return updatedNote;
        }

        public async Task<UserNote> GetUserNoteById(int id)
        {
            // Call the repository method to get the user note by ID
            var userNote = await _noteRepository.GetUserNoteById(id);

            // Return the retrieved user note
            return userNote;
        }

        public async Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorId(int collaboratorId)
        {
            // Call the repository method to get user notes by collaborator ID
            return await _noteRepository.GetUserNotesByCollaboratorId(collaboratorId);
        }


        /*        public async Task<bool> SendCollaboratorMessageAsync(string email)
                {
                    // Validate the email address (optional)
                    if (!IsValidEmail(email))
                    {
                        throw new ArgumentException("Invalid email address.");
                    }

                    // Send the message to the email address
                    try
                    {
                        await _emailService.SendEmailAsync(email, "Collaboration Invitation", "You have been invited as a collaborator.");
                        return true; // Email sent successfully
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        throw new Exception("Error sending email to collaborator.", ex);
                    }
                }*/

        private bool IsValidEmail(string email)
        {
            // Implement email validation logic (e.g., regex or using email validation libraries)
            // For simplicity, assuming all non-null and non-empty strings are valid emails
            return !string.IsNullOrEmpty(email);
        }

        public Task<bool> SendCollaboratorMessage(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteUserNoteByTitle(string title)
        {
            // Get the UserNoteId by title
            int userNoteId = await _noteRepository.GetUserNoteIdByTitle(title);
            if (userNoteId == 0)
            {
                // User note not found
                return false;
            }

            // Delete labels associated with the user note
            await _labelRepository.DeleteLabelsByUserNoteId(userNoteId);

            // Delete the user note
            return await _noteRepository.DeleteUserNote(userNoteId);
        }

        // Implement other business logic methods as needed



        public async Task<List<UserNote>> GetUserNotesByUserId(int userId)
        {
            return await _noteRepository.GetUserNotesByUserId(userId);
        }

    }
}

