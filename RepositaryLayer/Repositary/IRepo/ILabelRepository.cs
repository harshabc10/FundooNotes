using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ILabelRepository
    {
        public Task<ActionResult<LabelsRequest>> CreateLabel(LabelsRequest label);
        public Task<Label> EditLabel(Label label);
        public Task<int> DeleteLabel(int labelId);
        public Task<int> RemoveLabel(int userId, int noteId);
        public Task<List<Label>> GetUsersLabelsList(int userId);
        public Task<List<UserNote>> GetNotesByUserId(int userId);

        public Task<bool> DeleteLabelsByUserNoteId(int userNoteId);
    }
}
