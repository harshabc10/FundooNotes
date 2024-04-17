using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface ICollaboratorService
    {
        public Task<CollaboratorRequest> AddCollaborator(CollaboratorRequest collaborator);
        public Task<bool> DeleteCollaborator(int collaboratorId);
        public Task<Collaborator> GetCollaborator(int collaboratorId);
    }

}
