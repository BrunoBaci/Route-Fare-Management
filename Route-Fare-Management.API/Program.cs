using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Route_Fare_Management.API;
using Route_Fare_Management.API.Services;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.TourOperator.HAndlers;
using Route_Fare_Management.Infrastructure.Services;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//  Controllers 
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Infrastructure 
builder.Services.AddInfrastructure(builder.Configuration);

//  MediatR 
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateTourOperatorCommandHandler).Assembly);
});

// JWT Settings 
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings missing");

//  HttpContext 
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//  Authentication 
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

            ClockSkew = TimeSpan.FromMinutes(5)
        };

        // SignalR token support
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                var path = ctx.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(token) &&
                    path.StartsWithSegments("/hubs"))
                {
                    ctx.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IJwtService, JwtService>();
//  SignalR 
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

builder.Services.AddScoped<IHubClientsAdapter, HubClientsAdapter>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
//app.MapHub<HubClientsAdapter>("/hubs/notifications");

app.Run();