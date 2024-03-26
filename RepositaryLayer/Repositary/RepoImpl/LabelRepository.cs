﻿using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
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
            // Implement logic to insert a new label into the database
            using (var connection = _context.CreateConnection())
            {
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
    }
}
