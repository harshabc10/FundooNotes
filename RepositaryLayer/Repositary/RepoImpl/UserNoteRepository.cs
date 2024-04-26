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

        public async Task<UserNoteRequest> AddUserNote(string userId, UserNoteRequest note)
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
                    // Add the UserId parameter to the anonymous object for Dapper
                    var parameters = new
                    {
                        UserId = userId,
                        note.Title,
                        note.Description,
                        note.Color,
                        note.ImagePaths,
                        note.Reminder,
                        note.IsArchive,
                        note.IsPinned,
                        note.IsTrash
                    };

                    int id = await connection.ExecuteScalarAsync<int>(sql, parameters);

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


        public async Task<bool> ArchiveUserNote(string userId, int noteId)
        {
            try
            {
                // SQL query to update the IsArchive flag for the specified note
                string sql = "UPDATE UserNotes SET IsArchive = 1 WHERE Id = @NoteId AND UserId = @UserId";

                // Create an anonymous object containing parameter values for the query
                var parameters = new { NoteId = noteId, UserId = userId };

                // Execute the SQL query using Dapper and retrieve the number of affected rows
                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, parameters);

                    // Return true if at least one row was affected (note was archived successfully), false otherwise
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error archiving user note.", ex);
            }
        }

        public async Task<bool> TrashUserNote(string userId, int noteId)
        {
            try
            {
                // SQL query to update the IsTrash flag for the specified note
                string sql = "UPDATE UserNotes SET IsTrash = 1 WHERE Id = @NoteId AND UserId = @UserId";

                // Create an anonymous object containing parameter values for the query
                var parameters = new { NoteId = noteId, UserId = userId };

                // Execute the SQL query using Dapper and retrieve the number of affected rows
                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, parameters);

                    // Return true if at least one row was affected (note was trashed successfully), false otherwise
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error trashing user note.", ex);
            }
        }



        /*public async Task<bool> DeleteUserNote(int id)
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
        }*/

        public async Task<bool> DeleteUserNote(string userId, int noteId)
        {
            try
            {
                // SQL query to delete a user note from the UserNotes table based on the note ID and user ID
                string sql = "DELETE FROM UserNotes WHERE Id = @NoteId AND UserId = @UserId";

                // Create an anonymous object containing parameter values for the query
                var parameters = new { NoteId = noteId, UserId = userId };

                // Execute the SQL query using Dapper and retrieve the number of affected rows
                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, parameters);

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




        /*        public async Task<UserNote> UpdateUserNote(string userId, UserNote note)
                {
                    try
                    {
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

                        using (var connection = _context.CreateConnection())
                        {
                            await connection.ExecuteAsync(sql, note);
                        }

                        return note;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        throw new Exception("Error updating user note in the database.", ex);
                    }
                }*/

        public async Task<UserNote> UpdateUserNote(string userId, int noteId, UserNoteRequest noteRequest)
        {
            // Build the SQL query for updating the user note
            string sql = @"
                UPDATE UserNotes 
                SET 
                    Title = @Title,
                    Description = @Description,
                    Color = @Color,
                    ImagePaths = @ImagePaths,
                    Reminder = @Reminder,
                    IsArchive = @IsArchive,
                    IsPinned = @IsPinned,
                    IsTrash = @IsTrash
                WHERE Id = @Id AND UserId = @UserId";

            // Create an anonymous object containing parameter values for the query
            var parameters = new
            {
                Id = noteId,
                UserId = userId,
                Title = noteRequest.Title,
                Description = noteRequest.Description,
                Color = noteRequest.Color,
                ImagePaths = noteRequest.ImagePaths,
                Reminder = noteRequest.Reminder,
                IsArchive = noteRequest.IsArchive,
                IsPinned = noteRequest.IsPinned,
                IsTrash = noteRequest.IsTrash,
            };

            // Execute the update query using Dapper
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, parameters);
            }

            // Return the updated user note
            var updatedNote = await GetUserNoteById(noteId);
            return updatedNote;
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

        public async Task<bool> DeleteUserNotesById(string userId, int noteId)
        {
            // Implement logic to delete user notes by ID from the database using Dapper
            using (var connection = _context.CreateConnection())
            {
                string query = "DELETE FROM UserNotes WHERE UserId = @UserId AND Id = @NoteId";
                int affectedRows = await connection.ExecuteAsync(query, new { UserId = userId, NoteId = noteId });

                return affectedRows > 0;
            }
        }

        public async Task<UserNote> GetUserNotesById(string userId, int noteId)
        {
            // Implement logic to get user notes by ID from the database using Dapper
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT * FROM UserNotes WHERE UserId = @UserId AND Id = @NoteId";
                return await connection.QueryFirstOrDefaultAsync<UserNote>(query, new { UserId = userId, NoteId = noteId });
            }
        }

        public async Task<UserNoteRequest> UpdateUserNotesById(string userId, int noteId, UserNoteRequest note)
        {
            // Implement logic to update user notes by ID in the database using Dapper
            using (var connection = _context.CreateConnection())
            {
                string query = @"UPDATE UserNotes 
                 SET Title = @Title, 
                     Description = @Description, 
                     Color = @Color, 
                     ImagePaths = @ImagePaths, 
                     Reminder = @Reminder, 
                     IsArchive = @IsArchive, 
                     IsPinned = @IsPinned, 
                     IsTrash = @IsTrash 
                 WHERE UserId = @UserId AND Id = @NoteId";

                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    Title = note.Title,
                    Description = note.Description,
                    Color = note.Color,
                    ImagePaths = note.ImagePaths,
                    Reminder = note.Reminder,
                    IsArchive = note.IsArchive,
                    IsPinned = note.IsPinned,
                    IsTrash = note.IsTrash,
                    UserId = userId,
                    NoteId = noteId
                });

                if (affectedRows > 0)
                {
                    // If update is successful, fetch and return the updated note
                    string selectQuery = "SELECT * FROM UserNotes WHERE UserId = @UserId AND Id = @NoteId";
                    return await connection.QueryFirstOrDefaultAsync<UserNoteRequest>(selectQuery, new { UserId = userId, NoteId = noteId });
                }
                else
                {
                    // If no rows were affected, return null indicating the note was not updated
                    return null;
                }
            }
        }

    }
}
