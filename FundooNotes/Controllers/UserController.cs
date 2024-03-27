using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.DTO.RequestDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {   
        
        private readonly IUserService service ;
        private readonly IConfiguration _configuration ;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService service, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.service = service;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor; 
        }

        [HttpPost]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> createUser(UserRequest request)
        {
            
               return Ok($"User create sucessfull : {await service.createUser(request)}");
        }

        [HttpGet("Login/{Email}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> Login(string Email, string password)
        {
           var user =await service.Login(Email, password);

            //session creting
            _httpContextAccessor.HttpContext.Session.SetString("UserName", user.FirstName);

            var token = CreateToken(user);
            return Ok($"Token Generated sucessfully{new {Token = token}}");

        }


        [HttpPut("forgotpass/{Email}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePasswordRequest(String Email)
        {
            return Ok($"{await service.ChangePasswordRequest(Email)}");
        }

        [HttpPut("otp/{otp}/{password}")]
        [UserExceptionHandlerFilter]
        public async Task<IActionResult> ChangePassword(String otp,String password)
        {
            return Ok(await service.ChangePassword(otp,password));
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
        new Claim(ClaimTypes.Name, $"{user.FirstName}{user.LastName}")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(1);

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
