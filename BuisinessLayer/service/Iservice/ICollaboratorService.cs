using ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface ICollaboratorService
    {
        public Task<Collaborator> AddCollaborator(Collaborator collaborator);
        public Task<bool> DeleteCollaborator(int collaboratorId);
        public Task<Collaborator> GetCollaborator(int collaboratorId);
    }

}
