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
    [Authorize]

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

        [HttpPost]
        public async Task<ActionResult<LabelsRequest>> CreateLabel([FromBody] LabelsRequest labelRequest)
        {
            try
            {
                // Validate label request
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get the user ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Extract the user ID
                var userId = userIdClaim.Value;

                // Optionally, you can validate the user ID or perform additional checks here

                // Proceed with creating the label
                var createdLabel = await _labelService.CreateLabel(userId, labelRequest);
                var response = new ResponseModel<LabelsRequest>
                {
                    Message = "Label created successfully",
                    Data = labelRequest
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<LabelsRequest>
                {
                    Success = false,
                    Message = $"Error creating label: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }


        [HttpPut("{labelId}")]
        public async Task<IActionResult> EditLabelByUserId(int labelId, EditLabelRequestDto requestDto)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Create a Label entity with the provided data
                var label = new Label
                {
                    LabelId = labelId,
                    UserId = int.Parse(userId),
                LabelName = requestDto.LabelName,
                    NoteId = requestDto.NoteId
                };

                // Call the label service to update the label
                var updatedLabel = await _labelService.EditLabel(label);

                // Prepare the response
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



        [HttpDelete("{labelId}")]
        public async Task<IActionResult> DeleteLabelById(int labelId)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Call the label service to delete the label
                var isDeleted = await _labelService.DeleteLabelById(userId, labelId);

                if (isDeleted)
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
                var response = new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"Error deleting label: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet("{labelId}")]
        public async Task<IActionResult> GetLabelById(int labelId)
        {
            try
            {
                // Get the user ID from JWT claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized("User ID not found in claims.");
                }

                // Call the label service to get the label by ID
                var label = await _labelService.GetLabelById(userId, labelId);

                if (label != null)
                {
                    var response = new ResponseModel<Label>
                    {
                        Message = "Label retrieved successfully",
                        Data = label
                    };
                    return Ok(response);
                }
                else
                {
                    return NotFound("Label not found.");
                }
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<Label>
                {
                    Success = false,
                    Message = $"Error retrieving label: {ex.Message}"
                };
                return StatusCode(500, response);
            }
        }


    }
}
