using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Interface
{
    public interface ILabelRepository
    {
        public Task<ActionResult<LabelsRequest>> CreateLabel(string userId, LabelsRequest label);
        public Task<Label> EditLabel(Label label);

        Task<bool> DeleteLabelById(string userId, int labelId);
        Task<Label> GetLabelById(string userId, int labelId);


        public Task<List<Label>> GetUsersLabelsList(int userId);
        public Task<List<UserNote>> GetNotesByUserId(int userId);

        public Task<bool> DeleteLabelsByUserNoteId(int userNoteId);
    }
}
