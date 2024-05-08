using BuisinessLayer.CustomException;
using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.Interface;
using Confluent.Kafka;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Entity;
using ModelLayer.Models.RequestDto;
using ModelLayer.Models.ResponceDto;
using Newtonsoft.Json;
using RepositaryLayer.DTO.RequestDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    [EnableCors]
    public class UserController : ControllerBase
    {

        private readonly IUserService service;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService service, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.service = service;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            
        }

        [HttpPost]
        public async Task<IActionResult> createUser(UserRequest request)
        {
            try
            {
                var userId = await service.createUser(request); // Assuming service.createUser returns an int

                var userResponse = new ResponseModel<UserResponce>
                {
                    Message = "User created successfully.",
                    Data = new UserResponce // Assuming 'UserResponse' is the response model for user creation
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        Id=userId
                    }
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }





        /*        [HttpPost]
                [UserExceptionHandlerFilter]
                public async Task<IActionResult> createUser(UserRequest request)
                {

                       return Ok($"User create sucessfull : {await service.createUser(request)}");
                }*/

        [HttpPost("Login")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> Login(string Email, string password)
        {
            try
            {
                var user = await service.Login(Email, password);

                // Check if user is found
                if (user == null)
                {
                    return NotFound("User not found."); // You can customize this message as needed
                }

                // Create a session for the user
                _httpContextAccessor.HttpContext.Session.SetString("UserName", user.FirstName);

                // Generate JWT token
                var token = CreateToken(user);

                // Return token along with success message
                var response = new
                {
                    Token = token,
                    UserName = user.FirstName, // Assuming FirstName contains the username
                    Email = user.Email,
                    Message = "Token generated successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }


    [HttpPut("ForgotPassword")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePasswordRequest(String Email)
        {
            try
            {
                // Assuming service.ForgotPassword returns a string message
                var responseMessage = await service.ChangePasswordRequest(Email);
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = responseMessage,
                    Data = Email
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPost("ResetPassword")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePassword(String otp, String password)
        {
            try
            {
                // Assuming service.ResetPassword returns a string message
                var responseMessage = await service.ChangePassword(otp, password);
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = responseMessage
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                });
            }
        }

        //JWt
        /*private string CreateToken(UserResponce user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,$"{user.FirstName}{user.LastName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Tokens"]));

            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                 claims: claims,
                expires: expires,
                signingCredentials: cred

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/

        private string CreateToken(UserResponce user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
                new Claim(ClaimTypes.Email, $"{user.Email}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(100);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}
