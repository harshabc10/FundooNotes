using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Interface
{
    public interface ICollaboratorService
    {
        public Task<CollaboratorRequest> AddCollaborator(string userId, CollaboratorRequest collaborator);

        public Task<bool> DeleteCollaboratorById(string userId, int collaboratorId);

        public Task<Collaborator> GetCollaboratorById(string userId, int collaboratorId);


        /*        public Task<bool> DeleteCollaborator(int userId, int collaboratorId);
                public Task<Collaborator> GetCollaborator(int collaboratorId);*/
    }

}
