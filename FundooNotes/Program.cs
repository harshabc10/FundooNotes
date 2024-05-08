using BuisinessLayer.Interface;
using BuisinessLayer.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using NLog.Web;
using RepositaryLayer.Context;
using RepositaryLayer.Interface;
using RepositaryLayer.Service;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


//kafka implementations

// Register the ApacheKafkaConsumerService as a singleton hosted service
builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var producerConfig = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<string, string>(producerConfig).Build();
});

builder.Services.AddSingleton<IConsumer<string, string>>(sp =>
{
    var consumerConfig = new ConsumerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        GroupId = builder.Configuration["Kafka:ConsumerGroupId"],
        AutoOffsetReset = AutoOffsetReset.Earliest
    };
    return new ConsumerBuilder<string, string>(consumerConfig).Build();
});

//redis concept
/*builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "127.0.0.1:6379"; // Redis server address
    options.InstanceName = "FundooNotesCache"; // Instance name for cache keys
   
});*/
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>(); // Retrieve the IConfiguration object
    var redisConnectionString = configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://localhost:7004")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
            .AllowAnyOrigin();
        });
});



//---------------------------------------------------------------------------------------------------------------------------------------

//loggers
/*builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders(); // Clear default logging providers

    // Configure NLog
    logging.AddNLog(new NLogProviderOptions
    {
        CaptureMessageProperties = true,
        CaptureMessageTemplates = true
    });

    // Load NLog configuration from file
    string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nlog.config");
    NLog.LogManager.LoadConfiguration(configFilePath);
});*/

var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();
builder.Services.AddSingleton<NLog.ILogger>(NLog.LogManager.GetCurrentClassLogger());
//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Add services to the container. for user login
builder.Services.AddSingleton<DapperContext>();
//builder.Services.AddScoped<IUserRepo, UserRepoImpl>();
builder.Services.AddTransient<IUserRepo, UserRepoImpl>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();

//usernotes
builder.Services.AddScoped<IUserNoteRepository, UserNoteRepository>();
builder.Services.AddScoped<IUserNoteService, UserNoteService>();

//collbration
builder.Services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();
builder.Services.AddScoped<ICollaboratorService, CollaboratorService>();

//lables
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<ILabelService, LabelService>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//for acquring lock on swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Welcome To FundooNotes Environment", Version = "v1" });

    // Define the JWT bearer scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    // Require JWT tokens to be passed on requests
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDistributedMemoryCache();

//jwt

// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]));
//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,



        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key
    };
});


//Ending...
var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy =>
    {
        policy.WithOrigins("http://localhost:7254", "https://localhost:7254")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithHeaders(HeaderNames.ContentType);
    });
}


app.UseHttpsRedirection();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();