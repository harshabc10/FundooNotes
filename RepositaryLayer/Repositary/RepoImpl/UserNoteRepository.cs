using Dapper;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserNoteRepository : IUserNoteRepository
    {
        private readonly DapperContext _context;

        public UserNoteRepository(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserNoteRequest> AddUserNote(UserNoteRequest note)
        {
            try
            {
                // SQL query to insert a new user note into the UserNotes table
                string sql = @"
    INSERT INTO UserNotes (UserId, Title, Description, Color, ImagePaths, Reminder, IsArchive, IsPinned, IsTrash)
    VALUES (@UserId, @Title, @Description, @Color, @ImagePaths, @Reminder, @IsArchive, @IsPinned, @IsTrash);
    SELECT SCOPE_IDENTITY();"; // Retrieve the ID of the newly inserted record

                // Execute the SQL query using Dapper and retrieve the ID of the inserted record
                using (var connection = _context.CreateConnection())
                {
                    int id = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        note.UserId,
                        note.Title,
                        note.Description,
                        note.Color,
                        note.ImagePaths,
                        note.Reminder,
                        note.IsArchive,
                        note.IsPinned,
                        note.IsTrash
                    });

                    // Set the ID of the user note object
                    note.UserId = id;

                    // Return the modified user note object
                    return note;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error adding user note to the database.", ex);
            }
        }



        public async Task<bool> DeleteUserNote(int id)
        {
            try
            {
                // SQL query to delete a user note from the UserNotes table
                string sql = "DELETE FROM UserNotes WHERE Id = @Id";

                // Execute the SQL query using Dapper and retrieve the number of affected rows
                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

                    // Check if any rows were affected (i.e., if the user note was deleted)
                    bool isDeleted = affectedRows > 0;
                    return isDeleted;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error deleting user note from the database.", ex);
            }
        }



        public async Task<UserNote> UpdateUserNote(UserNote note)
        {
            try
            {
                // SQL query to update a user note in the UserNotes table
                string sql = @"
                    UPDATE UserNotes
                    SET Title = @Title,
                        Description = @Description,
                        Color = @Color,
                        ImagePaths = @ImagePaths,
                        Reminder = @Reminder,
                        IsArchive = @IsArchive,
                        IsPinned = @IsPinned,
                        IsTrash = @IsTrash
                    WHERE Id = @Id;";

                // Execute the SQL query using Dapper
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(sql, new
                    {
                        note.Id,
                        note.Title,
                        note.Description,
                        note.Color,
                        note.ImagePaths,
                        note.Reminder,
                        note.IsArchive,
                        note.IsPinned,
                        note.IsTrash
                    });

                    return note;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error updating user note in the database.", ex);
            }
        }

        public async Task<UserNote> GetUserNoteById(int id)
        {
            // SQL query to select a user note by ID from the UserNotes table
            string sql = "SELECT * FROM UserNotes WHERE Id = @Id;";

            // Execute the SQL query using Dapper and retrieve the user note
            using (var connection = _context.CreateConnection())
            {
                var userNote = await connection.QueryFirstOrDefaultAsync<UserNote>(sql, new { Id = id });
                return userNote;
            }
        }

        public async Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorId(int collaboratorId)
        {
            string query = "SELECT UN.Id, UN.Title, UN.Description, UN.Color, UN.ImagePaths, UN.Reminder, UN.IsArchive," +
                " UN.IsPinned, UN.IsTrash\r\nFROM UserNotes UN\r\nJOIN Collaborators C ON UN.Id = C.UserNoteId\r\nWHERE C.CollaboratorId = @CollaboratorId;";
            using var connection = _context.CreateConnection();
            var userNotes = await connection.QueryAsync<UserNote>(query, new { CollaboratorId = collaboratorId });
            return userNotes.ToList();
        }

        public async Task<bool> DeleteUserNoteByTitle(string title)
        {
            string sql = "DELETE FROM UserNotes WHERE Title = @Title";

            using var connection = _context.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, new { Title = title });

            return rowsAffected > 0;
        }

        public async Task<int> GetUserNoteIdByTitle(string title)
        {
            // Query to get the UserNoteId by title
            string query = "SELECT Id FROM UserNotes WHERE Title = @Title";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, new { Title = title });
        }

        public async Task<List<UserNote>> GetUserNotesByUserId(int userId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT * FROM UserNotes WHERE UserId = @UserId";
                var userNotes = await connection.QueryAsync<UserNote>(query, new { UserId = userId });
                return userNotes.ToList();
            }
        }
        // Implement other CRUD methods as needed
    }
}
