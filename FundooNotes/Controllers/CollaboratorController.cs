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
using ModelLayer.Models.RequestDto;
using ModelLayer.Models.ResponceDto;
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
        public async Task<ActionResult<CollaboratorRequest>> AddCollaborator(CollaboratorRequest collaborator)
        {
            try
            {
                var addedCollaborator = await _collaboratorService.AddCollaborator(collaborator);
                var response = new ResponseModel<CollaboratorRequest>
                {
                    Message = "Collaborator added successfully",
                    Data = addedCollaborator
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<CollaboratorRequest>
                {
                    Success = false,
                    Message = $"Error adding collaborator: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }


        [HttpDelete("ById")]
        public async Task<ActionResult<bool>> DeleteCollaborator(int collaboratorId)
        {
            try
            {
                var isDeleted = await _collaboratorService.DeleteCollaborator(collaboratorId);
                if (isDeleted)
                {
                    return Ok(true);
                }
                else
                {
                    return NotFound("Collaborator not found.");
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"Error deleting collaborator: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet("ById")]
        public async Task<ActionResult<Collaborator>> GetCollaborator(int collaboratorId)
        {
            try
            {
                var collaborator = await _collaboratorService.GetCollaborator(collaboratorId);
                if (collaborator == null)
                {
                    return NotFound();
                }
                return Ok(collaborator);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<Collaborator>
                {
                    Success = false,
                    Message = $"Error retrieving collaborator: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }

    }
}
