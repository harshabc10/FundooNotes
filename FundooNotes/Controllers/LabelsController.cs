using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RepositaryLayer.Entity;
using System.Text.Json;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly ILabelService _labelService;
        private readonly IDistributedCache _cache;

        public LabelsController(ILabelService labelService, IDistributedCache cache)
        {
            _labelService = labelService;
            _cache = cache;
        }

        [HttpPost]
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


        [HttpDelete("{userId}/{noteId}")]
        public async Task<IActionResult> RemoveLabel(int userId, int noteId)
        {
            try
            {
                int rowsAffected = await _labelService.RemoveLabel(userId, noteId);
                if (rowsAffected > 0)
                {
                    return Ok(true);
                }
                else
                {
                    return NotFound("Label not found on the note.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"Error removing label from note: {ex.Message}");
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

        [HttpGet("notes/{userId}")]
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

        //Redis concept
        [HttpGet("{userId}")]
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
        }

    }
}
