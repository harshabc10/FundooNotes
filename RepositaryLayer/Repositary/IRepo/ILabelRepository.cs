using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ILabelRepository
    {
        public Task<int> CreateLabel(Label label);
        public Task<int> EditLabel(Label label);
        public Task<int> DeleteLabel(int labelId);
        public Task<int> RemoveLabel(int userId, int noteId);
        public Task<List<Label>> GetUsersLabelsList(int userId);
        public Task<List<UserNote>> GetNotesByUserId(int userId);

        public Task<bool> DeleteLabelsByUserNoteIdAsync(int userNoteId);
    }
}
