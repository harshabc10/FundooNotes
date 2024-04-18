using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ICollaboratorRepository
    {
        public Task<CollaboratorRequest> AddCollaborator(string userId,CollaboratorRequest collaborator);

        Task<bool> DeleteCollaboratorById(string userId, int collaboratorId);

        Task<Collaborator> GetCollaboratorById(string userId, int collaboratorId);
        /*        public Task<bool> DeleteCollaborator(int userId, int collaboratorId);
                public Task<Collaborator> GetCollaborator(int collaboratorId);*/
    }

}
