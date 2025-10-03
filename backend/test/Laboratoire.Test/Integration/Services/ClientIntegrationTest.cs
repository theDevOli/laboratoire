using Dapper;
using Laboratoire.Application.Services.ClientServices;
using Laboratoire.Application.Services.UserServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class ClientIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public ClientIntegrationTest()
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
    public async Task ChangeClient_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new ClientRepository(_dbContext);
        var userRepository = new UserRepository(_dbContext);

        var userGetterByUsernameService =
        new UserGetterByUsernameService(userRepository, NullLogger<UserGetterByUsernameService>.Instance);
        var userRenameService = new UserRenameService(userRepository, NullLogger<UserRenameService>.Instance);

        var service =
        new ClientUpdatableService
        (repository, userGetterByUsernameService, userRenameService, NullLogger<ClientUpdatableService>.Instance);

        var clientId = await connection.ExecuteScalarAsync<Guid>
        (
            """
            WITH new_user AS (
                INSERT INTO users."user" (role_id, username, is_active)
                VALUES (@roleId, @username, @isActive)
                RETURNING user_id
            )
            INSERT INTO customers.client (
                client_name,
                user_id,
                client_tax_id,
                client_email,
                client_phone
            )
            SELECT 
                @clientName,user_id, @clientTaxId, @clientEmail,@clientPhone
            FROM new_user
            RETURNING client_id
            """,
            new
            {
                roleId = 1,
                username = "Test",
                isActive = true,
                clientName = "Test",
                clientTaxId = "00000000000",
                clientEmail = "test@email.com",
                clientPhone = "99000000000"
            }
        );

        Client toUpdate = new()
        {
            ClientId = clientId,
            ClientName = "Updated",
            ClientTaxId = "00000000000",
            ClientEmail = "test@email.com",
            ClientPhone = "99000000000"
        };

        // Act
        var response = await service.UpdateClientAsync(client: toUpdate);
        var updated = await repository.GetByClientIdAsync(clientId);
        Console.WriteLine(response.StatusCode);

        // Assert
        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updated);
        Assert.Equal(toUpdate.ClientName, updated.ClientName);

        await connection.ExecuteAsync("DELETE FROM customers.client WHERE client_id = @ClientId",new{ClientId = updated.ClientId});
        await connection.ExecuteAsync("DELETE FROM users.\"user\" WHERE user_id = @UserId",new{UserId = updated.UserId});
    }
}
