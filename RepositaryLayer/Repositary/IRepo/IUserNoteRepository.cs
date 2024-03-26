using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IUserNoteRepository
    {
        public Task<UserNote> AddUserNoteAsync(UserNote note);
        public Task<bool> DeleteUserNoteAsync(int id);
        public Task<UserNote> UpdateUserNoteAsync(UserNote note);
        public Task<UserNote> GetUserNoteByIdAsync(int id);
        public Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorIdAsync(int collaboratorId);
        // Add other CRUD methods as needed
    }
}
