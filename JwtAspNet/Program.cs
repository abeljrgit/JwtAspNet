using AutoMapper;
using JwtAspNet.Models;
using JwtAspNet.Repository;
using JwtAspNet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database Connection - Please check appsettings.json(Development/Production)
var dbConnectionStringType = builder.Environment.IsDevelopment() ? "DevJwtAspNetDbConnection" : builder.Environment.IsEnvironment("Uat") ? "UatJwtAspNetDbConnection" : "ProdJwtAspNetDbConnection";
var connectionString = builder.Configuration.GetConnectionString(dbConnectionStringType);

builder.Services.AddDbContext<JwtAspNetDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

// This will inject Automapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<IMapper, Mapper>();

// This will inject Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// This will inject Service
builder.Services.AddScoped<IUserService, UserService>();


// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // note the port is included 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// AllowAll CORS Policy
app.UseCors("MyAllowedOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
