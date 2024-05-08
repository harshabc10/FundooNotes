using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Interface
{
    public interface ILabelService
    {
        public Task<ActionResult<LabelsRequest>> CreateLabel(string userId, LabelsRequest label);
        public Task<Label> EditLabel(Label label);

        Task<bool> DeleteLabelById(string userId, int labelId);
        Task<Label> GetLabelById(string userId, int labelId);

        //need to implement
        public Task<List<Label>> GetUsersLabelsList(int userId);
        public Task<List<UserNote>> GetNotesByUserId(int userId);

    }
}
