using ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IUserNoteRepository
    {
        public Task<UserNote> AddUserNote(UserNote note);
        public Task<bool> DeleteUserNote(int id);
        public Task<UserNote> UpdateUserNote(UserNote note);
        public Task<UserNote> GetUserNoteById(int id);
        public Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorId(int collaboratorId);
        public Task<bool> DeleteUserNoteByTitle(string title);
        public Task<int> GetUserNoteIdByTitle(string title);
        // Add other CRUD methods as needed
    }
}
