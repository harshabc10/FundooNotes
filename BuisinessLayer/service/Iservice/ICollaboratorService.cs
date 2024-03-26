using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface ICollaboratorService
    {
        public Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator);
        public Task<bool> DeleteCollaboratorAsync(int collaboratorId);
        public Task<Collaborator> GetCollaboratorAsync(int collaboratorId);
    }

}
