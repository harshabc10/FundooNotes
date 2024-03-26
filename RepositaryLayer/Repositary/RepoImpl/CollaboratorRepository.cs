using Dapper;
using RepositaryLayer.Context;
using RepositaryLayer.Entity;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.RepoImpl
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly DapperContext _context;

        public CollaboratorRepository(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Collaborator> AddCollaboratorAsync(Collaborator collaborator)
        {
            try
            {
                string sql = @"
                    INSERT INTO Collaborators (UserId, UserNoteId, CollaboratorEmail)
                    VALUES (@UserId, @UserNoteId, @CollaboratorEmail);
                    SELECT SCOPE_IDENTITY();";

                using (var connection = _context.CreateConnection())
                {
                    int id = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        collaborator.UserId,
                        collaborator.UserNoteId,
                        collaborator.CollaboratorEmail
                    });

                    collaborator.CollaboratorId = id;
                    return collaborator;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error adding collaborator to the database.", ex);
            }
        }

        public async Task<bool> DeleteCollaboratorAsync(int collaboratorId)
        {
            try
            {
                string sql = "DELETE FROM Collaborators WHERE CollaboratorId = @CollaboratorId";

                using (var connection = _context.CreateConnection())
                {
                    int affectedRows = await connection.ExecuteAsync(sql, new { CollaboratorId = collaboratorId });
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error deleting collaborator from the database.", ex);
            }
        }

        public async Task<Collaborator> GetCollaboratorAsync(int collaboratorId)
        {
            try
            {
                string sql = "SELECT * FROM Collaborators WHERE CollaboratorId = @CollaboratorId";

                using (var connection = _context.CreateConnection())
                {
                    var collaborator = await connection.QueryFirstOrDefaultAsync<Collaborator>(sql, new { CollaboratorId = collaboratorId });
                    return collaborator;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("Error getting collaborator from the database.", ex);
            }
        }
    }
}
