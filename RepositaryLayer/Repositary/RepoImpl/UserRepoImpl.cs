using BuisinessLayer.Entity;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RepositaryLayer.Repositary.RepoImpl
{
    public class UserRepoImpl : IUserRepo
    {
        private readonly DapperContext context;
        private readonly ILogger<UserRepoImpl> logger; // Logger instance
        public UserRepoImpl(DapperContext contex, ILogger<UserRepoImpl> logger)
        {
            this.context = contex;
            this.logger = logger;
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
                logger.LogInformation("User created successfully. Rows affected: {0}", rowsAffected);
               
                return rowsAffected;
            }
            catch (Exception ex)
            {
                // Log the exception
                logger.LogError(ex, "Error occurred while creating user.");
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
            logger.LogInformation("Getting user by email: {0}", email);

            return await connection.QueryFirstAsync<UserEntity>(Query, new { Email = email });
        }

        public async Task<int> UpdatePassword(string mailid, string password)
        {
            String Query = "update Users set UserPassword = @Password where UserEmail = @mail";
            IDbConnection connection = context.CreateConnection();
            int rowsAffected = await connection.ExecuteAsync(Query, new { mail = mailid, Password = password });

            // Log information message
            logger.LogInformation("Password updated successfully for email: {0}. Rows affected: {1}", mailid, rowsAffected);

            return rowsAffected;
        }

    }
}
