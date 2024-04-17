/*using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.MailSender;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        private static string otp;
        private static string mailid;
        private static UserEntity entity;

        private readonly IConfiguration _configuration;
        public UserServiceImpl(IUserRepo UserRepo,IConfiguration configuration)
        {
            this.UserRepo = UserRepo;
            this._configuration = configuration;
        }
        private UserEntity MapToEntity(UserRequest request)
        {
            return new UserEntity
            {
                UserFirstName = request.FirstName,
                UserLastName = request.LastName,
                UserEmail = request.Email,
                UserPassword = Encrypt(request.Password)

            };
        }
        private String Encrypt(String password)
        {

            byte[] passByte = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passByte);

        }
        private String Decrypt(String encryptedPass)
        {

            byte[] passbyte = Convert.FromBase64String(encryptedPass);
            String res = Encoding.UTF8.GetString(passbyte);

            return res;

        }
        private UserResponce MapToResponce(UserEntity responce)
        {
            return new UserResponce
            {
                FirstName = responce.UserFirstName,
                LastName = responce.UserLastName,
                Email = responce.UserEmail,


            };
        }

        public Task<int> createUser(UserRequest request)
        {
            return UserRepo.createUser(MapToEntity(request));
        }

        public Task<UserResponce> Login(string Email, string password)
        {
            UserEntity entity;
            try
            {
                entity = UserRepo.GetUserByEmail(Email).Result;
            }
            catch (AggregateException e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId");
            }
            if (password.Equals(Decrypt(entity.UserPassword)))
            {
                return Task.FromResult(MapToResponce(entity));
            }
            else
            {
                Console.WriteLine(Decrypt(entity.UserPassword));
                Console.WriteLine(password);
                Console.WriteLine(password.Equals(Decrypt(entity.UserPassword)));
                throw new PasswordMissmatchException("Incorrect Password");
            }

        }

        public Task<String> ChangePasswordRequest(string Email)
        {
            try
            {
                entity = UserRepo.GetUserByEmail(Email).Result;
            }
            catch (Exception e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId" + e.Message);
            }

            string generatedotp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                generatedotp += r.Next(0, 10);
            }
            otp = generatedotp;
            mailid = Email;
            MailSenderClass.sendMail(Email, generatedotp);
            Console.WriteLine(otp);
            return Task.FromResult("MailSent ✔️");

        }




        public Task<string> ChangePassword(string otp, string password)
        {
            if (otp.Equals(null))
            {
                return Task.FromResult("Generate Otp First");
            }
            if (Decrypt(entity.UserPassword).Equals(password))
            {
                throw new PasswordMissmatchException("Dont give the existing password");
            }

            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                if (UserServiceImpl.otp.Equals(otp))
                {
                    if (UserRepo.UpdatePassword(mailid, Encrypt(password)).Result == 1)
                    {
                        entity = null; otp = null; mailid = null;
                        return Task.FromResult("password changed successfully");
                    }
                }
                else
                {
                    return Task.FromResult("otp miss matching");
                }
            }
            else
            {
                return Task.FromResult("regex is mismatching");
            }
            return Task.FromResult("password not changed");

        }

        public async Task<UserResponce> Authenticate(string email, string password)
        {
            // Check if user exists in the database
            var user = await UserRepo.GetUserByEmail(email);

            if (user == null || user.UserPassword != password)
            {
                // User not found or password incorrect
                return null;
            }

            // User authenticated successfully
            var token = CreateToken(user);

            // You can return additional user details or just the token
            return new UserResponce
            {
                
                FirstName = user.UserFirstName,
                LastName = user.UserLastName,
                Email = user.UserEmail,
               
            };
        }

        private String CreateToken(UserResponce user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,$"{user.FirstName}{user.LastName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Tokens"]));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                 claims: claims,
                expires: expires,
                signingCredentials: cred

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
*/

using BuisinessLayer.CustomException;
using BuisinessLayer.MailSender;
using BuisinessLayer.service.Iservice;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Entity;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        private static string otp;
        private static string mailid;
        private static UserEntity entity;

        private readonly IConfiguration _configuration;
        public UserServiceImpl(IUserRepo UserRepo, IConfiguration configuration)
        {
            this.UserRepo = UserRepo;
            _configuration = configuration;
        }

        private UserEntity MapToEntity(UserRequest request)
        {
            return new UserEntity
            {
                UserFirstName = request.FirstName,
                UserLastName = request.LastName,
                UserEmail = request.Email,
                UserPassword = Encrypt(request.Password)
            };
        }

        private String Encrypt(String password)
        {
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passByte);
        }

        private String Decrypt(String encryptedPass)
        {
            byte[] passbyte = Convert.FromBase64String(encryptedPass);
            String res = Encoding.UTF8.GetString(passbyte);
            return res;
        }

        private UserResponce MapToResponce(UserEntity response)
        {
            return new UserResponce
            {
                FirstName = response.UserFirstName,
                LastName = response.UserLastName,
                Email = response.UserEmail,
                Id=response.UserId
            };
        }

        public async Task<int> createUser(UserRequest request)
        {
            return await UserRepo.createUser(MapToEntity(request));
        }

        public async Task<UserResponce> Login(string Email, string password)
        {
            UserEntity entity;
            try
            {
                entity = await UserRepo.GetUserByEmail(Email);
            }
            catch (AggregateException e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId");
            }

            if (password.Equals(Decrypt(entity.UserPassword)))
            {
                return MapToResponce(entity);
            }
            else
            {
                Console.WriteLine(Decrypt(entity.UserPassword));
                Console.WriteLine(password);
                Console.WriteLine(password.Equals(Decrypt(entity.UserPassword)));
                throw new PasswordMissmatchException("Incorrect Password");
            }
        }

        public async Task<String> ChangePasswordRequest(string Email)
        {
            try
            {
                entity = await UserRepo.GetUserByEmail(Email);
            }
            catch (Exception e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId" + e.Message);
            }

            string generatedotp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                generatedotp += r.Next(0, 10);
            }
            otp = generatedotp;
            mailid = Email;
            MailSenderClass.sendMail(Email, generatedotp);
            Console.WriteLine(otp);
            return "MailSent ✔️";
        }

        public async Task<string> ChangePassword(string otp, string password)
        {
            if (otp == null)
            {
                return "Generate Otp First";
            }

            if (Decrypt(entity.UserPassword).Equals(password))
            {
                throw new PasswordMissmatchException("Don't give the existing password");
            }

            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                if (UserServiceImpl.otp.Equals(otp))
                {
                    if (await UserRepo.UpdatePassword(mailid, Encrypt(password)) == 1)
                    {
                        entity = null; otp = null; mailid = null;
                        return "Password changed successfully";
                    }
                }
                else
                {
                    return "OTP mismatch";
                }
            }
            else
            {
                return "Password does not meet the criteria";
            }
            return "Password not changed";
        }

        /* public async Task<UserResponce> Authenticate(string email, string password)
         {
             // Check if user exists in the database
             var user = await UserRepo.GetUserByEmail(email);

             if (user == null || user.UserPassword != password)
             {
                 // User not found or password incorrect
                 return null;
             }

             // User authenticated successfully
             var token = CreateToken(user);

             // You can return additional user details or just the token
             return new UserResponce
             {

                 FirstName = user.UserFirstName,
                 LastName = user.UserLastName,
                 Email = user.UserEmail,

             };
         }

         private string CreateToken(UserEntity user)
         {
             var claims = new[]
             {
                 new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                 new Claim(ClaimTypes.Name, $"{user.UserFirstName} {user.UserLastName}"),
                 new Claim(ClaimTypes.Email, user.UserEmail)
                 // Add more claims as needed
             };

             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Tokens"]));

             var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

             var expires = DateTime.UtcNow.AddHours(1);

             var token = new JwtSecurityToken(
                  claims: claims,
                 expires: expires,
                 signingCredentials: cred

                 );

             return new JwtSecurityTokenHandler().WriteToken(token);
         }*/
    }
}
