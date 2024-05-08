using BuisinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using RepositaryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuisinessLayer.Services
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<ActionResult<LabelsRequest>> CreateLabel(string userId, LabelsRequest label)
        {
            // Implement the logic to create a new label in the repository
            var createdLabel = await _labelRepository.CreateLabel(userId, label);
            return createdLabel;
        }

        public async Task<Label> EditLabel(Label label)
        {
            // Assuming _labelRepository.EditLabel returns the updated label
            var updatedLabel = await _labelRepository.EditLabel(label);
            return updatedLabel;
        }

        public async Task<bool> DeleteLabelById(string userId, int labelId)
        {
            return await _labelRepository.DeleteLabelById(userId, labelId);
        }

        public async Task<Label> GetLabelById(string userId, int labelId)
        {
            return await _labelRepository.GetLabelById(userId, labelId);
        }

        public Task<List<Label>> GetUsersLabelsList(int userId)
        {
            // Implement the logic to get a list of labels for a specific user from the repository
            return _labelRepository.GetUsersLabelsList(userId);
        }
        public async Task<List<UserNote>> GetNotesByUserId(int userId)
        {
            // Call the repository method to get notes by user ID
            return await _labelRepository.GetNotesByUserId(userId);
        }

    }
}
