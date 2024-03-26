using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;

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
        public async Task<ActionResult<Collaborator>> AddCollaboratorAsync(Collaborator collaborator)
        {
            var addedCollaborator = await _collaboratorService.AddCollaboratorAsync(collaborator);
            return Ok(addedCollaborator);
        }

        [HttpDelete("{collaboratorId}")]
        public async Task<ActionResult<bool>> DeleteCollaboratorAsync(int collaboratorId)
        {
            var isDeleted = await _collaboratorService.DeleteCollaboratorAsync(collaboratorId);
            return Ok(isDeleted);
        }

        [HttpGet("{collaboratorId}")]
        public async Task<ActionResult<Collaborator>> GetCollaboratorAsync(int collaboratorId)
        {
            var collaborator = await _collaboratorService.GetCollaboratorAsync(collaboratorId);
            if (collaborator == null)
            {
                return NotFound();
            }
            return Ok(collaborator);
        }
    }

}
