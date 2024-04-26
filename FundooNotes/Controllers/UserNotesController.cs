/*using BuisinessLayer.service.Iservice;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.ResponceDto;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;
using System;
using System.Security.Claims;

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

        */
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
                }*//*
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserNote>> AddUserNote(UserNote note)
        {
            try
            {
                var addedNote = await _noteService.AddUserNote(note);
                var response = new ResponseModel<UserNote>
                {
                    Message = "Note Created Successfully",
                    Data = addedNote
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNote>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response);
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

        [HttpGet("ByNoteId")]
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

        [HttpGet("ByUserId")]
        [Authorize]
        public async Task<ActionResult<UserNote>> GetUserNotesByUserId()
        {
            try
            {
                // Get the user ID from the claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return StatusCode(403, "Unauthorized: User ID claim not found or invalid.");
                }

                // Call the service method to get user notes by user ID
                var userNotes = await _noteService.GetUserNotesByUserId(userId);

                if (userNotes == null || userNotes.Count == 0)
                {
                    return NotFound("User notes not found.");
                }

                // Return the user notes as a response
                return Ok(userNotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user notes: {ex.Message}");
            }
        }

    }

}
*/


using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using ModelLayer.Models.ResponceDto;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FundooNotes.Controllers
{
    [ApiController]
    [Route("api/usernotes")]
    [Authorize]
    public class UserNotesController : ControllerBase
    {
        private readonly IUserNoteService _noteService;

        public UserNotesController(IUserNoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<UserNoteRequest>>> AddUserNote(UserNoteRequest note)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Add the note using the service, passing the user ID
                var addedNote = await _noteService.AddUserNote(userId, note);

                // Prepare the response
                var response = new ResponseModel<UserNoteRequest>
                {
                    Message = "Note Created Successfully",
                    Data = addedNote
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNoteRequest>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }



        [HttpGet("ByUserId")]
        public async Task<ActionResult<ResponseModel<UserNote>>> GetUserNotesByUserId()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return StatusCode(403, new ResponseModel<UserNote> { Message = "Unauthorized: User ID claim not found or invalid." });
                }

                // Here you're trying to return a list of UserNote objects, but the method signature expects a single UserNote
                var userNotes = await _noteService.GetUserNotesByUserId(userId);

                if (userNotes == null || userNotes.Count == 0)
                {
                    return NotFound(new ResponseModel<UserNote> { Message = "User notes not found" });
                }

                var response = new ResponseModel<List<UserNote>>
                {
                    Message = "User notes retrieved successfully",
                    Data = userNotes  // This line causes the error
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNote>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return StatusCode(500, response);
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> DeleteUserNotesById(int id)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Delete user notes by ID using the service
                var isDeleted = await _noteService.DeleteUserNotesById(userId, id);

                if (isDeleted)
                {
                    var response = new ResponseModel<bool>
                    {
                        Message = "User note deleted successfully",
                        Data = true
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound(new ResponseModel<bool> { Message = "User note not found or unauthorized" });
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }





        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<UserNote>>> GetUserNotesById(int id)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Retrieve user notes by ID using the service
                var userNote = await _noteService.GetUserNotesById(userId, id);

                if (userNote != null)
                {
                    var response = new ResponseModel<UserNote>
                    {

                        Message = "User note retrieved successfully",
                        Data = userNote
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound(new ResponseModel<UserNote> { Message = "User note not found or unauthorized" });
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNote>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<UserNoteRequest>>> UpdateUserNotesById(int id, UserNoteRequest note)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Update the note using the service, passing the user ID and note ID
                var updatedNote = await _noteService.UpdateUserNotesById(userId, id, note);

                if (updatedNote != null)
                {
                    var response = new ResponseModel<UserNoteRequest>
                    {
                        Message = "Note Updated Successfully",
                        Data = updatedNote
                    };
                    return Ok(response);
                }
                else
                {

                    return NotFound(new ResponseModel<UserNoteRequest> { Message = "User note not found or unauthorized" });
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNoteRequest>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }


        [HttpPost("archive")]
        public async Task<ActionResult<ResponseModel<bool>>> ArchiveUserNote(ArchiveTrashRequest request)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Call the service method to archive the user note
                bool isArchived = await _noteService.ArchiveUserNote(userId, request.NoteId);

                if (isArchived)
                {
                    var response = new ResponseModel<bool>
                    {
                        Message = "Note Archived Successfully",
                        Data = true
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound(new ResponseModel<bool> { Message = "User note not found" });
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost("trash")]
        public async Task<ActionResult<ResponseModel<bool>>> TrashUserNote(ArchiveTrashRequest request)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                bool isTrashed = await _noteService.TrashUserNote(userId, request.NoteId);

                if (isTrashed)
                {
                    var response = new ResponseModel<bool>
                    {
                        Message = "Note Trashed Successfully",
                        Data = true
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound(new ResponseModel<bool> { Message = "User note not found" });
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }

        }
    }

        //edit needed

        /*
                [HttpDelete("{noteId}")]
                public async Task<ActionResult<ResponseModel<bool>>> DeleteUserNote(int noteId)
                {
                    try
                    {
                        // Get the user ID from JWT claims
                        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (userId == null)
                        {
                            return Unauthorized("User ID not found in claims.");
                        }

                        // Call the service method to delete the user note
                        bool isDeleted = await _noteService.DeleteUserNote(userId, noteId);

                        if (isDeleted)
                        {
                            var response = new ResponseModel<bool>
                            {
                                Message = "Note Deleted Successfully",
                                Data = true
                            };
                            return Ok(response);
                        }
                        else
                        {
                            return NotFound(new ResponseModel<bool> { Message = "User note not found" });
                        }
                    }
                    catch (Exception ex)
                    {
                        var response = new ResponseModel<bool>
                        {
                            Success = false,
                            Message = ex.Message
                        };
                        return StatusCode(500, response);
                    }
                }

                [HttpGet("ByNoteId")]
                public async Task<ActionResult<ResponseModel<UserNote>>> GetUserNoteById(int id)
                {
                    try
                    {
                        var userNote = await _noteService.GetUserNoteById(id);

                        if (userNote == null)
                        {
                            return NotFound(new ResponseModel<UserNote> { Message = "User note not found" });
                        }

                        var response = new ResponseModel<UserNote>
                        {
                            Message = "User note retrieved successfully",
                            Data = userNote
                        };
                        return Ok(response);
                    }
                    catch (Exception ex)
                    {
                        var response = new ResponseModel<UserNote>
                        {
                            Success = false,
                            Message = ex.Message,
                        };
                        return StatusCode(500, response);
                    }
                }*/


    }


/*
        [HttpDelete("{noteId}")]
        public async Task<ActionResult<ResponseModel<bool>>> DeleteUserNote(int noteId)
        {
            try
            {
                bool isDeleted = await _noteService.DeleteUserNote(noteId);
                if (isDeleted)
                {
                    var response = new ResponseModel<bool>
                    {
                        Message = "Note Deleted Successfully",
                        Data = true
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound(new ResponseModel<bool> { Message = "User note not found" });
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return StatusCode(500, response);
            }
        }*/

/*        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<UserNoteRequest>>> UpdateUserNote(int id, UserNoteRequest note)
        {
            try
            {
                // Get the user ID from JWT claims
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("User ID not found in claims or invalid format.");
                }

                // Create a UserNote object with the provided data
                var userNote = new UserNote
                {
                    Id = id,
                    UserId = userId,
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color,
                    ImagePaths = note.ImagePaths,
                    Reminder = note.Reminder,
                    IsArchive = note.IsArchive,
                    IsPinned = note.IsPinned,
                    IsTrash = note.IsTrash
                };

                // Update the user note using the service, passing the user ID
                var updatedNote = await _noteService.UpdateUserNote(userNote);

                // Prepare the response
                var response = new ResponseModel<UserNoteRequest>
                {
                    Message = "Note Updated Successfully",
                    Data = updatedNote
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNoteRequest>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }*/

/*        [HttpPut("{noteId}")]
        public async Task<ActionResult<ResponseModel<UserNoteRequest>>> UpdateUserNote(int noteId, UserNoteRequest noteRequest)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Call the service method to update the user note
                var updatedNote = await _noteService.UpdateUserNote(userId, noteId, noteRequest);

                // Prepare the response
                var response = new ResponseModel<UserNoteRequest>
                {
                    Message = "Note Updated Successfully",
                    Data = noteRequest
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<UserNoteRequest>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }*/