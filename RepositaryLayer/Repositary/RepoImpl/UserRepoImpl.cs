using ModelLayer.Entity;
using Dapper;
using Microsoft.Extensions.Logging;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ILogger = NLog.ILogger;


namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserRepoImpl : IUserRepo
    {
        private readonly DapperContext context;
        private readonly ILogger logger; // Logger instance
        public UserRepoImpl(DapperContext contex, ILogger<UserRepoImpl> logger)
        {
            this.context = contex;
            this.logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<int> createUser(UserEntity entity)
        {
            try
            {
                string query = "insert into Users values (@UserFirstName, @UserLastName, @UserEmail, @UserPassword)";
                var connection = context.CreateConnection();

                //return await connection.ExecuteAsync(query, entity);
                int rowsAffected = await connection.ExecuteAsync(query, entity);

                // Log information message
                logger.Info("User created successfully. Rows affected: {0}", rowsAffected);
               
                return rowsAffected;
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.Error(ex, "Error occurred while creating user.");
                throw; // Rethrow the exception
            }
        }

        /*        public async Task<UserEntity> GetUserByEmail(string email)
                {
                    String Query = "Select * from Users where UserEmail = @Email";
                    IDbConnection connection = context.CreateConnection();

                       return await connection.QueryFirstAsync<UserEntity>(Query, new { Email = email });



                }

                public async Task<int> UpdatePassword(string mailid, string password)
                {
                    String Query = "update Users set UserPassword = @Password where UserEmail = @mail";
                    IDbConnection connection= context.CreateConnection();
                   return await connection.ExecuteAsync(Query,new {mail=mailid,Password=password});
                }*/

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            String Query = "Select * from Users where UserEmail = @Email";
            IDbConnection connection = context.CreateConnection();

            // Log information message
            logger.Info("Getting user by email: {0}", email);

            return await connection.QueryFirstAsync<UserEntity>(Query, new { Email = email });
        }

        public async Task<int> UpdatePassword(string mailid, string password)
        {
            String Query = "update Users set UserPassword = @Password where UserEmail = @mail";
            IDbConnection connection = context.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(Query, new { mail = mailid, Password = password });

            // Log information message
            logger.Info("Password updated successfully for email: {0}. Rows affected: {1}", mailid, rowsAffected);

            return rowsAffected;
        }

    }
}
