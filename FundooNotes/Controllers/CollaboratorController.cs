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

using BuisinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using ModelLayer.Models.ResponceDto;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [ApiController]
    [Route("api/collaborators")]
    [Authorize]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;

        public CollaboratorController(ICollaboratorService collaboratorService)
        {
            _collaboratorService = collaboratorService ?? throw new ArgumentNullException(nameof(collaboratorService));
        }

        [HttpPost]
        public async Task<ActionResult<CollaboratorRequest>> AddCollaborator([FromBody] CollaboratorRequest collaborator)
        {
            try
            {
                // Get the user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Extract the user ID
                var userId = userIdClaim.Value;

                // Optionally, you can validate the user ID or perform additional checks here

                // Proceed with adding collaborator
                var addedCollaborator = await _collaboratorService.AddCollaborator(userId, collaborator);
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
                    Message = $"The Note ID or User ID does not exist.: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<string>>> DeleteCollaboratorById(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                var isDeleted = await _collaboratorService.DeleteCollaboratorById(userId, id);
                if (isDeleted)
                {
                    var response = new ResponseModel<string>
                    {
                        Message = "Collaborator deleted successfully",
                        Data = "true"
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound("Collaborator not found");
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Error deleting collaborator: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<Collaborator>>> GetCollaboratorById(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                var collaborator = await _collaboratorService.GetCollaboratorById(userId, id);
                if (collaborator != null)
                {
                    var response = new ResponseModel<Collaborator>
                    {
                        Message = "Collaborator retrieved successfully",
                        Data = collaborator
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound("Collaborator not found");
                }
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
