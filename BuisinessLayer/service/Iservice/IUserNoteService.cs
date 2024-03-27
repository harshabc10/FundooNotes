using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface IUserNoteService
    {
        public Task<UserNote> AddUserNoteAsync(UserNote note);
        public Task<bool> DeleteUserNoteAsync(int id);
        public Task<UserNote> UpdateUserNoteAsync(UserNote note);
        public Task<UserNote> GetUserNoteByIdAsync(int id);
        public Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorIdAsync(int collaboratorId);

        public Task<bool> SendCollaboratorMessageAsync(string email);
        public Task<bool> DeleteUserNoteByTitleAsync(string title);
        // Add other business logic methods as needed
    }

}
