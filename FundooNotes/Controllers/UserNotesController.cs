using BuisinessLayer.service.Iservice;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;
using System;

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
        public async Task<ActionResult<UserNote>> AddUserNote(UserNote note)
        {
            try
            {
                var addedNote = await _noteService.AddUserNote(note);
                return Ok(addedNote); // Return 200 OK with the added note
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding user note: {ex.Message}");
            }
        }

        [HttpDelete("ById")]
        public async Task<ActionResult<bool>> DeleteUserNote(int id)
        {
            try
            {
                bool isDeleted = await _noteService.DeleteUserNote(id);
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
        public async Task<ActionResult<UserNote>> UpdateUserNote(UserNote note)
        {
            try
            {
                // Validate input
                if (note == null || note.Id <= 0)
                {
                    return BadRequest("Invalid user note data.");
                }

                // Call the service method to update the user note
                var updatedNote = await _noteService.UpdateUserNote(note);

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

        [HttpGet("ByUserId")]
        [Authorize]
        public async Task<ActionResult<UserNote>> GetUserNoteById(int id)
        {
            try
            {
                // Call the service method to get the user note by ID
                var userNote = await _noteService.GetUserNoteById(id);

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

    }

}
