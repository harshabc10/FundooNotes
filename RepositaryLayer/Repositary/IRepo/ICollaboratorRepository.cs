using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ICollaboratorRepository
    {
        public Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator);
        public Task<bool> DeleteCollaboratorAsync(int collaboratorId);
        public Task<Collaborator> GetCollaboratorAsync(int collaboratorId);
    }

}
