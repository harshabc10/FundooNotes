using ModelLayer.Entity;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface IUserNoteService
    {
        public Task<UserNoteRequest> AddUserNote(UserNoteRequest note);
        public Task<bool> DeleteUserNote(int id);
        public Task<UserNote> UpdateUserNote(UserNote note);
        public Task<UserNote> GetUserNoteById(int id);
        public Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorId(int collaboratorId);

        public Task<bool> SendCollaboratorMessage(string email);
        public Task<bool> DeleteUserNoteByTitle(string title);

        public Task<List<UserNote>> GetUserNotesByUserId(int userId);
        // Add other business logic methods as needed
    }

}
