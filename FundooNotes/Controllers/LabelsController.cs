using BuisinessLayer.service.Iservice;
using BuisinessLayer.service.serviceImpl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using ModelLayer.Models.ResponceDto;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]

    public class LabelsController : ControllerBase
    {
        private readonly ILabelService _labelService;

        public LabelsController(ILabelService labelService)
        {
            _labelService = labelService;
        }

        /*        [HttpPost]
                public async Task<IActionResult> CreateLabel(Label label)
                {
                    try
                    {
                        var createdLabel = await _labelService.CreateLabel(label);
                        return Ok(createdLabel);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error creating label: {ex.Message}");
                    }
                }

                [HttpPut("{labelId}")]
                public async Task<IActionResult> EditLabel(int labelId, Label label)
                {
                    try
                    {
                        label.LabelId = labelId; // Set the labelId from the route parameter
                        var updatedLabel = await _labelService.EditLabel(label);
                        if (updatedLabel != null)
                        {
                            return Ok(updatedLabel);
                        }
                        else
                        {
                            return NotFound("Label not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error updating label: {ex.Message}");
                    }
                }

                [HttpDelete("{labelId}")]
                public async Task<IActionResult> DeleteLabel(int labelId)
                {
                    try
                    {
                        int rowsAffected = await _labelService.DeleteLabel(labelId);
                        if (rowsAffected > 0)
                        {
                            return Ok(true);
                        }
                        else
                        {
                            return NotFound("Label not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error deleting label: {ex.Message}");
                    }
                }

        */


        /*        //redis implementation to these apis

                [HttpPost]
                public async Task<IActionResult> CreateLabel(Label label)
                {
                    try
                    {
                        var createdLabel = await _labelService.CreateLabel(label);

                        // Cache the created label
                        var cacheKey = $"Label_{createdLabel}";

                       await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(createdLabel));

                        return Ok(createdLabel);
                    }
                    catch (Exception ex)
                    {
                        // Log and handle the exception
                        return StatusCode(500, $"Error creating label: {ex.Message}");
                    }
                }

                [HttpPut("ById")]
                public async Task<IActionResult> EditLabel(int labelId, Label label)
                {
                    try
                    {
                        label.LabelId = labelId; // Set the labelId from the route parameter
                        var updatedLabel = await _labelService.EditLabel(label);

                        // Update cached label if it exists
                        var cacheKey = $"Label_{labelId}";
                        var cachedLabel = await _cache.GetStringAsync(cacheKey);
                        if (!string.IsNullOrEmpty(cachedLabel))
                        {
                            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(updatedLabel));
                        }

                        if (updatedLabel != null)
                        {
                            return Ok(updatedLabel);
                        }
                        else
                        {
                            return NotFound("Label not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error updating label: {ex.Message}");
                    }
                }

                [HttpDelete("ById")]
                public async Task<IActionResult> DeleteLabel(int labelId)
                {
                    try
                    {
                        int rowsAffected = await _labelService.DeleteLabel(labelId);

                        // Remove cached label if it exists
                        var cacheKey = $"Label_{labelId}";
                        var cachedLabel = await _cache.GetStringAsync(cacheKey);
                        if (!string.IsNullOrEmpty(cachedLabel))
                        {
                            await _cache.RemoveAsync(cacheKey);
                        }

                        if (rowsAffected > 0)
                        {
                            return Ok(true);
                        }
                        else
                        {
                            return NotFound("Label not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error deleting label: {ex.Message}");
                    }
                }

                [HttpGet("ByUserId")]
                public async Task<IActionResult> GetNotesByUserId(int userId)
                {
                    try
                    {
                        var notes = await _labelService.GetNotesByUserId(userId);
                        if (notes != null && notes.Count > 0)
                        {
                            return Ok(notes);
                        }
                        else
                        {
                            return NotFound("No notes found for the specified user ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error retrieving notes for user ID {userId}: {ex.Message}");
                    }
                }

                [HttpGet("ByUserId")]
                public async Task<IActionResult> GetUsersLabelsList(int userId)
                {
                    var cacheKey = $"Labels_{userId}";
                    var cachedLabels = await _cache.GetStringAsync(cacheKey);

                    if (!string.IsNullOrEmpty(cachedLabels))
                    {
                        // Return cached data if available
                        return Ok(JsonSerializer.Deserialize<List<Label>>(cachedLabels));
                    }

                    // Cache data if not already cached
                    var labels = await _labelService.GetUsersLabelsList(userId);
                    if (labels != null)
                    {
                        var cacheOptions = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache expiration time
                        };

                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(labels), cacheOptions);
                        return Ok(labels);
                    }

                    return NotFound("No labels found for the specified user ID.");
                }*/

        [HttpPost("CreateLabel")]


        public async Task<ActionResult<ResponseModel<LabelsRequest>>> CreateLabel(LabelsRequest label)
        {
            if (label == null)
            {
                return BadRequest("Label data is required.");
            }

            // You can add more validation logic here if needed

            try
            {
                // Insert label into the database using your repository
                var result = await _labelService.CreateLabel(label);

                // Check if the insertion was successful
                if (result!=null)
                {
                    var response = new ResponseModel<LabelsRequest>
                    {
                        Data = label,
                        Message = "Label created successfully."
                        // You can include more properties in the response model as needed
                    };
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "Failed to create label. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // You can customize the error message based on the exception type
                return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
            }
        }






        [HttpPut("EditLabel/{labelId}")]
        public async Task<IActionResult> EditLabel(int labelId, Label label)
        {
            try
            {
                label.LabelId = labelId;
                var updatedLabel = await _labelService.EditLabel(label);
                var response = new ResponseModel<Label>
                {
                    Message = "Label updated successfully",
                    Data = updatedLabel
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<Label>
                {
                    Success = false,
                    Message = $"Error updating label: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }

        [HttpDelete("DeleteLabel/{labelId}")]
        public async Task<IActionResult> DeleteLabel(int labelId)
        {
            try
            {
                int rowsAffected = await _labelService.DeleteLabel(labelId);
                if (rowsAffected > 0)
                {
                    var response = new ResponseModel<bool>
                    {
                        Message = "Label deleted successfully",
                        Data = true
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseModel<bool>
                    {
                        Success = false,
                        Message = "Label not found"
                    };
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"Error deleting label: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }

        [HttpDelete("RemoveLabel")]
        public async Task<IActionResult> RemoveLabel(int userId, int noteId)
        {
            try
            {
                int rowsAffected = await _labelService.RemoveLabel(userId, noteId);
                if (rowsAffected > 0)
                {
                    var response = new ResponseModel<bool>
                    {
                        Message = "Label removed from note successfully",
                        Data = true
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseModel<bool>
                    {
                        Success = false,
                        Message = "Label not found on the note"
                    };
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"Error removing label from note: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }


        /*        [HttpGet("{userId}")]
                public async Task<IActionResult> GetUsersLabelsList(int userId)
                {
                    try
                    {
                        var labels = await _labelService.GetUsersLabelsList(userId);
                        return Ok(labels);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return StatusCode(500, $"Error getting user's labels: {ex.Message}");
                    }
                }*/




    }
}
