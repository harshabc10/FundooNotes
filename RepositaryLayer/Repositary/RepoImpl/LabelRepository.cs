/*using Dapper;
using Microsoft.Data.SqlClient;
using ModelLayer.Entity;
using ModelLayer.Entity;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary;
using RepositaryLayer.Repositary.IRepo;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class LabelRepository : ILabelRepository
    {
        private readonly DapperContext _context;

        public LabelRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateLabel(Label label)
        {
            using (var connection = _context.CreateConnection())
            {
                // Check if the NoteId exists in the UserNotes table
                var note = await connection.QueryFirstOrDefaultAsync<UserNote>(
                    "SELECT * FROM UserNotes WHERE Id = @NoteId", new { NoteId = label.NoteId });

                if (note == null)
                {
                    // NoteId does not exist in UserNotes table, handle error or return appropriate response
                    return 0; // Return 0 indicating failure
                }

                // NoteId exists, proceed with inserting the label
                string query = "INSERT INTO Labels (LabelName, UserId, NoteId) VALUES (@LabelName, @UserId, @NoteId)";
                return await connection.ExecuteAsync(query, label);
            }
        }

        public async Task<int> EditLabel(Label label)
        {
            // Implement logic to update an existing label in the database
            using (var connection = _context.CreateConnection())
            {
                string query = "UPDATE Labels SET LabelName = @LabelName WHERE LabelId = @LabelId";
                return await connection.ExecuteAsync(query, label);
            }
        }

        public async Task<int> DeleteLabel(int labelId)
        {
            // Implement logic to delete a label from the database
            using (var connection = _context.CreateConnection())
            {
                string query = "DELETE FROM Labels WHERE LabelId = @LabelId";
                return await connection.ExecuteAsync(query, new { LabelId = labelId });
            }
        }

        public async Task<int> RemoveLabel(int userId, int noteId)
        {
            // Implement logic to remove a label from a note
            using (var connection = _context.CreateConnection())
            {
                string query = "DELETE FROM Labels WHERE UserId = @UserId AND NoteId = @NoteId";
                return await connection.ExecuteAsync(query, new { UserId = userId, NoteId = noteId });
            }
        }

        public async Task<List<Label>> GetUsersLabelsList(int userId)
        {
            // Implement logic to fetch all labels for a user from the database
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT * FROM Labels WHERE UserId = @UserId";
                return (await connection.QueryAsync<Label>(query, new { UserId = userId })).ToList();
            }
        }

        public async Task<List<UserNote>> GetNotesByUserId(int userId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT UN.Id AS NoteId, UN.Title, UN.Description, UN.Color, UN.ImagePaths, UN.Reminder," +
                    " UN.IsArchive, UN.IsPinned, UN.IsTrash\r\nFROM UserNotes UN\r\nJOIN Labels L ON UN.Id = L.NoteId\r\nWHERE L.UserId = @UserId;";
                return (await connection.QueryAsync<UserNote>(query, new { UserId = userId })).ToList();
            }
        }
        public async Task<bool> DeleteLabelsByUserNoteId(int userNoteId)
        {
            using (var connection = _context.CreateConnection())
            {
                //await connection.OpenAsync(); // Open the SqlConnection asynchronously

                var affectedRows = await connection.ExecuteAsync(
                    "DELETE FROM Labels WHERE NoteId = @UserNoteId",
                    new { UserNoteId = userNoteId });

                return affectedRows > 0;
            }
        }
    }
}
*/

//redis

using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using ModelLayer.Models.ResponceDto;
using Newtonsoft.Json;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class LabelRepository : ILabelRepository
    {
        private readonly DapperContext _context;
        private readonly IDatabase _cache;
        private readonly IConnectionMultiplexer _redis;

        public LabelRepository(DapperContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
            _cache = _redis.GetDatabase();
        }
        public async Task<ActionResult<LabelsRequest>> CreateLabel(string userId, LabelsRequest label)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    // Insert the new label into the database
                    string query = @"
                INSERT INTO Labels (LabelName, UserId, NoteId)
                VALUES (@LabelName, @UserId, @NoteId);
                SELECT SCOPE_IDENTITY();";

                    int labelId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        label.LabelName,
                        label.NoteId
                    });


                    // Clear cache after creating a new label
                    string cacheKey = $"UserLabels:{userId}";
                    await _cache.KeyDeleteAsync(cacheKey);

                    // Return the created label
                    return label;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding label to the database.", ex);
            }
        }






        public async Task<Label> EditLabel(Label label)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "UPDATE Labels SET LabelName = @LabelName WHERE LabelId = @LabelId";
                int affectedRows = await connection.ExecuteAsync(query, label);

                // Clear cache after editing a label
                if (affectedRows > 0)
                {
                    string cacheKey = $"UserLabels:{label.UserId}";
                    await _cache.KeyDeleteAsync(cacheKey);
                }

                // Assuming your repository method returns the updated label
                return label;
            }
        }

        public async Task<bool> DeleteLabelById(string userId, int labelId)
        {
            // Implement logic to delete a label from the database
            using (var connection = _context.CreateConnection())
            {
                string query = "DELETE FROM Labels WHERE UserId = @UserId AND LabelId = @LabelId";
                int affectedRows = await connection.ExecuteAsync(query, new { UserId = userId, LabelId = labelId });

                // Clear cache after deleting a label
                if (affectedRows > 0)
                {
                    string cacheKey = $"UserLabels:{labelId}";
                    await _cache.KeyDeleteAsync(cacheKey);
                }

                return affectedRows > 0; // Return true if rows were affected, false otherwise
            }
        }

        public async Task<Label> GetLabelById(string userId, int labelId)
        {
            // Implement retrieval logic using Dapper or your preferred method
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT * FROM Labels WHERE UserId = @UserId AND LabelId = @LabelId";
                return await connection.QueryFirstOrDefaultAsync<Label>(query, new { UserId = userId, LabelId = labelId });
            }
        }


        public async Task<List<Label>> GetUsersLabelsList(int userId)
        {
            string cacheKey = $"UserLabels:{userId}";
            string cacheValue = await _cache.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cacheValue))
            {
                // Cache hit, return cached data
                return DeserializeLabels(cacheValue);
            }

            // Cache miss, fetch data from database
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT * FROM Labels WHERE UserId = @UserId";
                var labels = (await connection.QueryAsync<Label>(query, new { UserId = userId })).ToList();

                // Cache the data for future requests
                await _cache.StringSetAsync(cacheKey, SerializeLabels(labels));

                return labels;
            }
        }

        public async Task<List<UserNote>> GetNotesByUserId(int userId)
        {
            string cacheKey = $"UserNotes:{userId}";
            string cacheValue = await _cache.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cacheValue))
            {
                // Cache hit, return cached data
                return DeserializeNotes(cacheValue);
            }

            // Cache miss, fetch data from database
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT UN.Id AS NoteId, UN.Title, UN.Description, UN.Color, UN.ImagePaths\r\n" +
            "FROM UserNotes UN\r\n" +
            "JOIN Labels L ON UN.Id = L.NoteId\r\n" +
            "WHERE L.UserId = @UserId;";
                var notes = (await connection.QueryAsync<UserNote>(query, new { UserId = userId })).ToList();

                // Cache the data for future requests
                await _cache.StringSetAsync(cacheKey, SerializeNotes(notes));

                return notes;
            }
        }

        public async Task<bool> DeleteLabelsByUserNoteId(int userNoteId)
        {
            using (var connection = _context.CreateConnection())
            {
                //await connection.OpenAsync(); // Open the SqlConnection asynchronously

                var affectedRows = await connection.ExecuteAsync(
                    "DELETE FROM Labels WHERE NoteId = @UserNoteId",
                    new { UserNoteId = userNoteId });

                // Clear cache after deleting labels by user note ID
                if (affectedRows > 0)
                {
                    string cacheKey = $"UserLabels:{userNoteId}";
                    await _cache.KeyDeleteAsync(cacheKey);
                }

                return affectedRows > 0;
            }
        }

        private string SerializeLabels(List<Label> labels)
        {
            // Implement serialization logic (e.g., JSON serialization)
            return JsonConvert.SerializeObject(labels);
        }

        private List<Label> DeserializeLabels(string serializedData)
        {
            // Implement deserialization logic (e.g., JSON deserialization)
            return JsonConvert.DeserializeObject<List<Label>>(serializedData);
        }

        private string SerializeNotes(List<UserNote> notes)
        {
            return JsonConvert.SerializeObject(notes);
        }

        private List<UserNote> DeserializeNotes(string serializedData)
        {
            return JsonConvert.DeserializeObject<List<UserNote>>(serializedData);
        }
    }
}
