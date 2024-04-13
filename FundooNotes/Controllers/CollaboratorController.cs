/*using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;

namespace FundooNotes.Controllers
{
    [ApiController]
    [Route("api/collaborators")]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;


        public CollaboratorController(ICollaboratorService collaboratorService)
        {
            _collaboratorService = collaboratorService ?? throw new ArgumentNullException(nameof(collaboratorService));
        }

        [HttpPost]
        public async Task<ActionResult<Collaborator>> AddCollaborator(Collaborator collaborator)
        {
            var addedCollaborator = await _collaboratorService.AddCollaborator(collaborator);
            return Ok(addedCollaborator);
        }

        [HttpDelete("ById")]
        public async Task<ActionResult<bool>> DeleteCollaborator(int collaboratorId)
        {
            var isDeleted = await _collaboratorService.DeleteCollaborator(collaboratorId);
            return Ok(isDeleted);
        }

        [HttpGet("ById")]
        public async Task<ActionResult<Collaborator>> GetCollaborator(int collaboratorId)
        {
            var collaborator = await _collaboratorService.GetCollaborator(collaboratorId);
            if (collaborator == null)
            {
                return NotFound();
            }
            return Ok(collaborator);
        }
    }

}
*/

using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using System;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [ApiController]
    [Route("api/collaborators")]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;

        public CollaboratorController(ICollaboratorService collaboratorService)
        {
            _collaboratorService = collaboratorService ?? throw new ArgumentNullException(nameof(collaboratorService));
        }

        [HttpPost]
        public async Task<ActionResult<Collaborator>> AddCollaborator(Collaborator collaborator)
        {
            // Simulate a CPU-bound operation using Task.Run
            var addedCollaborator = await Task.Run(() => _collaboratorService.AddCollaborator(collaborator));
            return Ok(addedCollaborator);
        }

        [HttpDelete("ById")]
        public async Task<ActionResult<bool>> DeleteCollaborator(int collaboratorId)
        {
            // Simulate a CPU-bound operation using Task.Run
            var isDeleted = await Task.Run(() => _collaboratorService.DeleteCollaborator(collaboratorId));
            return Ok(isDeleted);
        }

        [HttpGet("ById")]
        public async Task<ActionResult<Collaborator>> GetCollaborator(int collaboratorId)
        {
            // Simulate a CPU-bound operation using Task.Run
            var collaborator = await Task.Run(() => _collaboratorService.GetCollaborator(collaboratorId));
            if (collaborator == null)
            {
                return NotFound();
            }
            return Ok(collaborator);
        }
    }
}
