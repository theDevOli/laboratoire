using System.Security.Cryptography;
using Dapper;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class AuthIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public AuthIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        _connectionString = _config.GetConnectionString("DefaultConnectionDev")!;

        _dbContext = new DataContext(_config);
    }

    [Fact]
    public async Task ChangeUserPassword_ShouldSucceed()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new AuthRepository(_dbContext);
        var hasher = new PasswordHasher(_config);
        var service = new AuthChangePasswordService(repository, hasher, NullLogger<AuthChangePasswordService>.Instance);

        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        var hash = hasher.HashPassword("123", salt);
        var userId = await connection.ExecuteScalarAsync<Guid>(
            """
            WITH new_user AS (
                INSERT INTO users."user" (role_id, username, is_active)
                VALUES (1, 'Test', true)
                RETURNING user_id
            )
            INSERT INTO users.auth (user_id, password_salt, password_hash)
            SELECT user_id, @Salt, @Hash
            FROM new_user
            RETURNING user_id;
            """,
            new { Salt = salt, Hash = hash }
        );

        var dto = new UserDtoChangePassword
        {
            UserId = userId,
            OldPassword = "123",
            UserPassword = "456"
        };

        // Act
        var result = await service.ChangeUserPasswordAsync(dto);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Equal(0, result.StatusCode);

        var updated = await connection.QuerySingleAsync<Auth>(
            """
            SELECT 
                user_id as UserId,
                password_salt as password_salt,
                password_hash as PasswordHash
            FROM
                users.auth 
            WHERE 
                user_id = @UserId
            """,
            new { UserId = userId }
        );

        await connection.ExecuteAsync("DELETE FROM users.auth WHERE user_id = @userId", new { userId });
        await connection.ExecuteAsync("DELETE FROM users.\"user\" WHERE user_id = @userId", new { userId });

        Assert.NotEqual(hash, updated.Password.PasswordHash);
    }
}
