using BuisinessLayer.service.Iservice;
using ModelLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public Task<int> CreateLabel(Label label)
        {
            // Implement the logic to create a new label in the repository
            return _labelRepository.CreateLabel(label);
        }

        public Task<int> EditLabel(Label label)
        {
            // Implement the logic to edit an existing label in the repository
            return _labelRepository.EditLabel(label);
        }

        public Task<int> DeleteLabel(int labelId)
        {
            // Implement the logic to delete a label by its ID in the repository
            return _labelRepository.DeleteLabel(labelId);
        }

        public Task<int> RemoveLabel(int userId, int noteId)
        {
            // Implement the logic to remove a label from a user's note in the repository
            return _labelRepository.RemoveLabel(userId, noteId);
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
