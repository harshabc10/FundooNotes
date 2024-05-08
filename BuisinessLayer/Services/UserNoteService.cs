using BuisinessLayer.Interface;
using Google.Apis.Gmail.v1;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using RepositaryLayer.Interface;
using RepositaryLayer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Services
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

        public async Task<UserNoteRequest> AddUserNote(string userId, UserNoteRequest note)
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
                var addedNote = await _noteRepository.AddUserNote(userId, userNoteToAdd);
                return addedNote;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error adding user note to the database.", ex);
            }
        }


        public async Task<bool> DeleteUserNote(string userId, int id)
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
                bool isDeleted = await _noteRepository.DeleteUserNote(userId, id);
                return isDeleted;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error deleting user note.", ex);
            }
        }

        /*        public async Task<UserNote> UpdateUserNote(string userId, int noteId, UserNoteRequest noteRequest)
                {
                    // Validate input
                    if (note == null || note.Id <= 0)
                    {
                        throw new ArgumentException("Invalid user note data.");
                    }

                    // Check if the user note with the specified ID exists and belongs to the user
                    var existingNote = await _noteRepository.UpdateUserNote(userId, note);
                    if (existingNote == null)
                    {
                        throw new KeyNotFoundException("User note not found or unauthorized to update.");
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

                    // Call the repository method to update the user note in the database
                    var updatedNote = await _noteRepository.UpdateUserNote(existingNote);

                    return updatedNote;
                }*/

        public async Task<UserNote> UpdateUserNote(string userId, int noteId, UserNoteRequest noteRequest)
        {
            // Fetch the user note from the repository using Dapper
            var existingNote = await _noteRepository.GetUserNoteById(noteId);

            if (existingNote == null)
            {
                throw new InvalidOperationException("User note not found.");
            }

            // Map properties from noteRequest to existingNote
            existingNote.Title = noteRequest.Title;
            existingNote.Description = noteRequest.Description;
            existingNote.Color = noteRequest.Color;
            existingNote.ImagePaths = noteRequest.ImagePaths;
            existingNote.Reminder = noteRequest.Reminder;
            existingNote.IsArchive = noteRequest.IsArchive;
            existingNote.IsPinned = noteRequest.IsPinned;
            existingNote.IsTrash = noteRequest.IsTrash;

            // Call the repository method to update the user note using Dapper
            var updatedNote = await _noteRepository.UpdateUserNote(userId, noteId, noteRequest);

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

        /*        public async Task<bool> DeleteUserNoteByTitle(string title)
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
                }*/

        // Implement other business logic methods as needed



        public async Task<List<UserNote>> GetUserNotesByUserId(int userId)
        {
            return await _noteRepository.GetUserNotesByUserId(userId);
        }

        public Task<bool> DeleteUserNoteByTitle(string title)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> DeleteUserNotesById(string userId, int noteId)
        {
            // Implement logic to delete user notes by ID using the repository
            return await _noteRepository.DeleteUserNotesById(userId, noteId);
        }

        public async Task<UserNote> GetUserNotesById(string userId, int noteId)
        {
            // Implement logic to get user notes by ID using the repository
            return await _noteRepository.GetUserNotesById(userId, noteId);
        }
        public async Task<UserNoteRequest> UpdateUserNotesById(string userId, int noteId, UserNoteRequest note)
        {
            // Implement logic to update user notes by ID using the repository
            return await _noteRepository.UpdateUserNotesById(userId, noteId, note);
        }

        public async Task<bool> ArchiveUserNote(string userId, int noteId)
        {
            try
            {
                // Fetch the user note from the repository using Dapper
                var existingNote = await _noteRepository.GetUserNoteById(noteId);

                if (existingNote == null)
                {
                    throw new InvalidOperationException("User note not found.");
                }

                // Check if the note belongs to the requesting user
                if (existingNote.UserId.ToString() != userId)
                {
                    throw new UnauthorizedAccessException("Unauthorized: User does not have permission to archive this note.");
                }

                // Update the IsArchive flag to true
                existingNote.IsArchive = true;

                // Call the repository method to update the user note using Dapper
                var isArchived = await _noteRepository.ArchiveUserNote(userId, noteId);

                return isArchived;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error archiving user note.", ex);
            }
        }



        public async Task<bool> TrashUserNote(string userId, int noteId)
        {
            try
            {
                // Fetch the user note from the repository using Dapper
                var existingNote = await _noteRepository.GetUserNoteById(noteId);

                if (existingNote == null)
                {
                    throw new InvalidOperationException("User note not found.");
                }

                // Check if the note belongs to the requesting user
                if (existingNote.UserId.ToString() != userId)
                {
                    throw new UnauthorizedAccessException("Unauthorized: User does not have permission to trash this note.");
                }

                // Update the IsTrashed flag to true
                existingNote.IsTrash = true;

                // Call the repository method to update the user note using Dapper
                var isTrashed = await _noteRepository.TrashUserNote(userId, noteId);

                return isTrashed;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error trashing user note.", ex);
            }
        }

        public async Task<UserNoteRequest> ChangeNoteColor(string userId, int noteId, string color)
        {
            try
            {
                // Fetch the user note from the repository using Dapper
                var existingNote = await _noteRepository.GetUserNoteById(noteId);

                if (existingNote == null)
                {
                    throw new InvalidOperationException("User note not found.");
                }

                // Check if the note belongs to the requesting user
                if (existingNote.UserId.ToString() != userId)
                {
                    throw new UnauthorizedAccessException("Unauthorized: User does not have permission to change color for this note.");
                }

                // Update the color of the existing note
                existingNote.Color = color;

                // Call the repository method to update the user note color using Dapper
                var updatedNote = await _noteRepository.ChangeNoteColor(userId, noteId, color);

                // Map the updated note to the request DTO
                var updatedNoteRequest = new UserNoteRequest
                {
                    Title = updatedNote.Title,
                    Description = updatedNote.Description,
                    Color = updatedNote.Color,
                    ImagePaths = updatedNote.ImagePaths,
                    Reminder = updatedNote.Reminder,
                    IsArchive = updatedNote.IsArchive,
                    IsPinned = updatedNote.IsPinned,
                    IsTrash = updatedNote.IsTrash
                };

                return updatedNoteRequest;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error changing note color.", ex);
            }
        }

    }
}

