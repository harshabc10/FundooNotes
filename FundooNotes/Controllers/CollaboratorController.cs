using BuisinessLayer.service.Iservice;
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

        [HttpPost("CreateCollaborator")]
        public async Task<ActionResult<Collaborator>> AddCollaborator(Collaborator collaborator)
        {
            var addedCollaborator = await _collaboratorService.AddCollaborator(collaborator);
            return Ok(addedCollaborator);
        }

        [HttpDelete("DeleteByCollaboratorId")]
        public async Task<ActionResult<bool>> DeleteCollaborator(int collaboratorId)
        {
            var isDeleted = await _collaboratorService.DeleteCollaborator(collaboratorId);
            return Ok(isDeleted);
        }

        [HttpGet("GetByCollaboratorId")]
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
