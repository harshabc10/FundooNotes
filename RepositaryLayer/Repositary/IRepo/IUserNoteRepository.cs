using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IUserNoteRepository
    {
        public Task<UserNoteRequest> AddUserNote(string userId, UserNoteRequest note);
        public Task<bool> DeleteUserNote(string userId, int noteId);
        public Task<UserNote> UpdateUserNote(string userId, int noteId, UserNoteRequest noteRequest);
        public Task<UserNote> GetUserNoteById(int id);
        public Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorId(int collaboratorId);
        public Task<bool> DeleteUserNoteByTitle(string title);
        public Task<int> GetUserNoteIdByTitle(string title);

        public Task<List<UserNote>> GetUserNotesByUserId(int userId);
        // Add other CRUD methods as needed

        Task<bool> DeleteUserNotesById(string userId, int noteId);
        Task<UserNote> GetUserNotesById(string userId, int noteId);
        Task<UserNoteRequest> UpdateUserNotesById(string userId, int noteId, UserNoteRequest note);
    }
}
