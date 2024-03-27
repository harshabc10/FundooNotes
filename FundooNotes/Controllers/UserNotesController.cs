using BuisinessLayer.service.Iservice;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
using System;
using Microsoft.AspNetCore.Authorization;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;

namespace FundooNotes.Controllers
{
    [ApiController]
    [Route("api/usernotes")]
    public class UserNotesController : ControllerBase
    {

        private readonly IUserNoteService _noteService;
       

        public UserNotesController(IUserNoteService noteService)
        {
            _noteService = noteService;
            
        }

        /*        [HttpPost]
                public async Task<ActionResult<UserNote>> AddUserNoteAsync(UserNote note)
                {
                    try
                    {

                        var addedNote = await _noteService.AddUserNoteAsync(note);
                        return Ok(addedNote); // Return 200 OK with the added note
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding user note: {ex.Message}");
                    }
                }*/
        [HttpPost]
        public async Task<ActionResult<UserNote>> AddUserNoteAsync(UserNote note)
        {
            try
            {
                var addedNote = await _noteService.AddUserNoteAsync(note);
                return Ok(addedNote); // Return 200 OK with the added note
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding user note: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteUserNoteAsync(int id)
        {
            try
            {
                bool isDeleted = await _noteService.DeleteUserNoteAsync(id);
                if (isDeleted)
                {
                    return Ok(true); // Return 200 OK with true indicating successful deletion
                }
                else
                {
                    return NotFound(); // Return 404 Not Found if the user note with the given ID was not found
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting user note: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult<UserNote>> UpdateUserNoteAsync(UserNote note)
        {
            try
            {
                // Validate input
                if (note == null || note.Id <= 0)
                {
                    return BadRequest("Invalid user note data.");
                }

                // Call the service method to update the user note
                var updatedNote = await _noteService.UpdateUserNoteAsync(note);

                if (updatedNote == null)
                {
                    return NotFound("User note not found.");
                }

                // Return the updated user note as a response
                return Ok(updatedNote);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"Error updating user note: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserNote>> GetUserNoteByIdAsync(int id)
        {
            try
            {
                // Call the service method to get the user note by ID
                var userNote = await _noteService.GetUserNoteByIdAsync(id);

                if (userNote == null)
                {
                    return NotFound("User note not found.");
                }

                // Return the user note as a response
                return Ok(userNote);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"Error retrieving user note: {ex.Message}");
            }
        }


        [HttpPost("addcollaborator")]
        public async Task<IActionResult> AddCollaboratorAsync([FromBody] string email)
        {
            try
            {
                // Call the collaborator service method to send a message to the email
                bool emailSent = await _noteService.SendCollaboratorMessageAsync(email);

                if (emailSent)
                {
                    return Ok("Message sent to collaborator.");
                }
                else
                {
                    return BadRequest("Failed to send message to collaborator.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred.");
            }
        }

        [HttpGet("collaborator/{collaboratorId}")]
        public async Task<ActionResult<IEnumerable<UserNote>>> GetUserNotesByCollaboratorIdAsync(int collaboratorId)
        {
            try
            {
                // Call the service method to get user notes by collaborator ID
                var userNotes = await _noteService.GetUserNotesByCollaboratorIdAsync(collaboratorId);

                if (userNotes == null || !userNotes.Any())
                {
                    return NotFound("No user notes found for the collaborator.");
                }

                // Return the user notes as a response
                return Ok(userNotes);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving user notes by collaborator ID: {ex.Message}");
            }
        }

        [HttpDelete("delete/{title}")]
        public async Task<ActionResult<bool>> DeleteUserNoteByTitleAsync(string title)
        {
            try
            {
                bool isDeleted = await _noteService.DeleteUserNoteByTitleAsync(title);
                if (isDeleted)
                {
                    return Ok(true); // Return 200 OK with true indicating successful deletion
                }
                else
                {
                    return NotFound(); // Return 404 Not Found if the user note with the given title was not found
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting user note by title: {ex.Message}");
            }


            // Implement other API endpoints as needed
        }
    }

}
