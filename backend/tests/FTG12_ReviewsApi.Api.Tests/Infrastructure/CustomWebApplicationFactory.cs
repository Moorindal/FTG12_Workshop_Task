using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentMigrator.Runner.Initialization;
using FTG12_ReviewsApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace FTG12_ReviewsApi.Api.Tests.Infrastructure;

/// <summary>
/// Custom WebApplicationFactory providing isolated in-memory SQLite databases per test class
/// and helper methods for creating authenticated HTTP clients.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _keepAliveConnection;
    private readonly string _connectionString = $"DataSource=Test_{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _keepAliveConnection = new SqliteConnection(_connectionString);
            _keepAliveConnection.Open();

            services.RemoveAll(typeof(SqliteConnection));
            services.AddSingleton(_keepAliveConnection);

            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connectionString));

            services.RemoveAll(typeof(IConnectionStringAccessor));
            services.AddSingleton<IConnectionStringAccessor>(
                new TestConnectionStringAccessor(_connectionString));
        });

        builder.UseEnvironment("Development");
    }

    /// <summary>
    /// Creates an HttpClient authenticated with a JWT token for the specified user.
    /// </summary>
    public HttpClient CreateAuthenticatedClient(
        int userId = 1,
        string username = "Admin",
        string role = "Admin")
    {
        var client = CreateClient();
        var token = GenerateJwtToken(userId, username, role);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateJwtToken(int userId, string username, string role)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("FTG12-Training-Secret-Key-Min-32-Chars!!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: "FTG12_ReviewsApi",
            audience: "FTG12_ReviewsApp",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _keepAliveConnection?.Close();
            _keepAliveConnection?.Dispose();
        }
        base.Dispose(disposing);
    }

    private sealed class TestConnectionStringAccessor(string connectionString) : IConnectionStringAccessor
    {
        public string ConnectionString { get; } = connectionString;
    }
}
