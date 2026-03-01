using System.Text;
using FTG12_ReviewsApi.Application;
using FTG12_ReviewsApi.Application.Common.Interfaces;
using FTG12_ReviewsApi.Infrastructure;
using FTG12_ReviewsApi.Infrastructure.Services;
using FTG12_ReviewsApi.Middleware;
using FTG12_ReviewsApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Register controller services for dependency injection.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Configure JWT Bearer authentication.
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

// Configure CORS to allow the React frontend origin.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:7200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register the built-in ASP.NET Core health checks.
builder.Services.AddHealthChecks();

var app = builder.Build();

// Run FluentMigrator migrations and enable foreign keys.
app.Services.UseInfrastructure();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable CORS middleware — must be called before MapControllers.
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map the built-in health check endpoint at /healthz (ASP.NET convention).
app.MapHealthChecks("/healthz");

app.Run();
