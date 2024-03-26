using BuisinessLayer.Entity;
using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
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

        public async Task<UserNote> AddUserNoteAsync(UserNote note)
        {
            try
            {
                // SQL query to insert a new user note into the UserNotes table
                string sql = @"
            INSERT INTO UserNotes (Title, Description, Color, ImagePaths, Reminder, IsArchive, IsPinned, IsTrash)
            VALUES (@Title, @Description, @Color, @ImagePaths, @Reminder, @IsArchive, @IsPinned, @IsTrash);
            SELECT SCOPE_IDENTITY();"; // Retrieve the ID of the newly inserted record

                // Execute the SQL query using Dapper and retrieve the ID of the inserted record
                using (var connection = _context.CreateConnection())
                {
                    int id = await connection.ExecuteScalarAsync<int>(sql, new
                    {
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
                    note.Id = id;

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


        public async Task<bool> DeleteUserNoteAsync(int id)
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



        public async Task<UserNote> UpdateUserNoteAsync(UserNote note)
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

        public async Task<UserNote> GetUserNoteByIdAsync(int id)
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

        public async Task<IEnumerable<UserNote>> GetUserNotesByCollaboratorIdAsync(int collaboratorId)
        {
            string query = "SELECT UN.Id, UN.Title, UN.Description, UN.Color, UN.ImagePaths, UN.Reminder, UN.IsArchive," +
                " UN.IsPinned, UN.IsTrash\r\nFROM UserNotes UN\r\nJOIN Collaborators C ON UN.Id = C.UserNoteId\r\nWHERE C.CollaboratorId = @CollaboratorId;";
            using var connection = _context.CreateConnection();
            var userNotes = await connection.QueryAsync<UserNote>(query, new { CollaboratorId = collaboratorId });
            return userNotes.ToList();
        }
        // Implement other CRUD methods as needed
    }
}
