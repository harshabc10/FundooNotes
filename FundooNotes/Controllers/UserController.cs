using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositaryLayer.DTO.RequestDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService service;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProducer<string, string> _kafkaProducer;
        private readonly IConsumer<string, string> _kafkaConsumer;
        private CancellationTokenSource _cancellationTokenSource;

        public UserController(IUserService service, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.service = service;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            // Initialize Kafka producer
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };
            _kafkaProducer = new ProducerBuilder<string, string>(producerConfig).Build();

            // Initialize Kafka consumer
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:ConsumerGroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            // Subscribe to Kafka topic
            _kafkaConsumer.Subscribe(_configuration["Kafka:Topic"]);

            // Initialize cancellation token source for stopping the consumer
            _cancellationTokenSource = new CancellationTokenSource();

            // Start Kafka consumer background task
            Task.Run(() => ConsumeKafkaMessages(_cancellationTokenSource.Token));
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> createUser(UserRequest request)
        {
            try
            {
                var result = await service.createUser(request);

                // Produce Kafka message
                var message = $"User created: {result}";
                var kafkaMessage = new Message<string, string>
                {
                    Key = "user_created",
                    Value = message
                };
                await _kafkaProducer.ProduceAsync(_configuration["Kafka:Topic"], kafkaMessage);

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        private async Task ConsumeKafkaMessages(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _kafkaConsumer.Consume(cancellationToken);
                    var message = consumeResult.Message.Value;
                    Console.WriteLine($"Received Kafka message: {message}");
                }
            }
            catch (OperationCanceledException)
            {
                // Consumer cancellation requested
            }
            finally
            {
                _kafkaConsumer.Close();
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _kafkaProducer?.Dispose();
                _kafkaConsumer?.Dispose();
                _cancellationTokenSource?.Dispose();
            }
        }


        /*        [HttpPost]
                [UserExceptionHandlerFilter]
                public async Task<IActionResult> createUser(UserRequest request)
                {

                       return Ok($"User create sucessfull : {await service.createUser(request)}");
                }*/

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
