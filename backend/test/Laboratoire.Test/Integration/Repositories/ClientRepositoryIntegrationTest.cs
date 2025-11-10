using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class ClientRepositoryIntegrationTest
{
    private readonly ClientRepository _repository;
    private readonly UserRepository _userRepository;
    private readonly NpgsqlConnection _connection;

    public ClientRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new ClientRepository(dbContext);
        _userRepository = new UserRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllClientsAsync_ShouldSucceed()
    {
        // Arrange
        var filter = "ClientName";

        // Act
        var result = await _repository.GetAllClientsAsync(filter);

        // Assert
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<Client>>(result);
    }

    [Fact]
    public async Task GetClientsLikeAsync_ShouldSucceed()
    {
        // Arrange
        var filter = "ClientName";
        var firstClient = (await _repository.GetAllClientsAsync(filter)).FirstOrDefault();

        // Act
        var result = await _repository.GetClientsLikeAsync(firstClient!.ClientTaxId);
        // Assert
        Assert.NotEmpty(result);
        Assert.IsAssignableFrom<IEnumerable<Client>>(result);
    }

    [Fact]
    public async Task GetByClientIdAsync_ShouldSucceed()
    {
        // Arrange
        var filter = "ClientName";
        var firstClient = (await _repository.GetAllClientsAsync(filter)).FirstOrDefault();

        // Act
        var result = await _repository.GetByClientIdAsync(firstClient!.ClientId);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<Client>(result);
    }

    [Fact]
    public async Task GetByTaxIdAsync_ShouldSucceed()
    {
        // Arrange
        var filter = "ClientName";
        var firstClient = (await _repository.GetAllClientsAsync(filter)).FirstOrDefault();

        // Act
        var result = await _repository.GetByTaxIdAsync(firstClient!.ClientTaxId);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<Client>(result);
    }

    [Fact]
    public async Task DoesClientExistByClientIdAsync_ShouldSucceed()
    {
        // Arrange
        var filter = "ClientName";
        var firstClient = (await _repository.GetAllClientsAsync(filter)).FirstOrDefault();

        // Act
        var result = await _repository.DoesClientExistByClientIdAsync(firstClient!);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesClientExistByTaxIdAsync_ShouldSucceed()
    {
        // Arrange
        var filter = "ClientName";
        var firstClient = (await _repository.GetAllClientsAsync(filter)).FirstOrDefault();

        // Act
        var result = await _repository.DoesClientExistByTaxIdAsync(firstClient!);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddClientAsync_ShouldSucceed()
    {
        // Arrange
        var newUser = new User()
        {
            UserId = default,
            RoleId = 4,
            Username = "Test",
            IsActive = false
        };

        var userId = await _userRepository.AddUserAsync(newUser);
        var newClient = new Client()
        {
            ClientId = default,
            UserId = userId,
            ClientName = "Test",
            ClientTaxId = "00011122289",
            ClientEmail = "test@email.com",
            ClientPhone = "Test",
        };

        // Act
        var result = await _repository.AddClientAsync(newClient, userId);

        // Assert
        Assert.True(result);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM customers.client
            WHERE client_tax_id = @ClientTaxId
            AND client_email = @ClientEmail;
            """,
            new
            {
                ClientTaxId = newClient.ClientTaxId,
                ClientEmail = newClient.ClientEmail
            }
        );

        await _connection.ExecuteAsync
        (
            """
            DELETE FROM users."user"
            WHERE user_id = @UserId;
            """,
            new
            {
                UserId = userId
            }
        );
    }

    [Fact]
    public async Task UpdateClientAsync_ShouldSucceed()
    {
        // Arrange
        var newUser = new User()
        {
            UserId = default,
            RoleId = 4,
            Username = "Test",
            IsActive = false
        };

        var userId = await _userRepository.AddUserAsync(newUser);
        var newClient = new Client()
        {
            ClientId = default,
            UserId = userId,
            ClientName = "Test",
            ClientTaxId = "00011122289",
            ClientEmail = "test@email.com",
            ClientPhone = "Test",
        };
        await _repository.AddClientAsync(newClient, userId);
        var clientId = await _connection.QueryFirstAsync<Guid>
        (
            """
            SELECT 
                client_id
            FROM 
                customers.client
            WHERE 
                client_tax_id = @ClientTaxId
                AND client_email = @ClientEmail;
            """,
            new
            {
                ClientTaxId = newClient.ClientTaxId,
                ClientEmail = newClient.ClientEmail
            }
        );

        var toUpdateClient = new Client()
        {
            ClientId = clientId,
            UserId = userId,
            ClientName = "Updated",
            ClientTaxId = "00000000011",
            ClientEmail = "updated@email.com",
            ClientPhone = "Updated",
        };
        // Act
        var result = await _repository.UpdateClientAsync(toUpdateClient);
        var updatedClient = await _repository.GetByClientIdAsync(clientId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateClient.ClientId, updatedClient?.ClientId);
        Assert.Equal(toUpdateClient.UserId, updatedClient?.UserId);
        Assert.Equal(toUpdateClient.ClientName, updatedClient?.ClientName);
        Assert.Equal(toUpdateClient.ClientTaxId, updatedClient?.ClientTaxId);
        Assert.Equal(toUpdateClient.ClientEmail, updatedClient?.ClientEmail);
        Assert.Equal(toUpdateClient.ClientPhone, updatedClient?.ClientPhone);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM customers.client
            WHERE client_tax_id = @ClientTaxId
            AND client_email = @ClientEmail
            """,
            new
            {
                ClientTaxId = toUpdateClient.ClientTaxId,
                ClientEmail = toUpdateClient.ClientEmail
            }
        );

        await _connection.ExecuteAsync
        (
            """
            DELETE FROM users."user"
            WHERE user_id = @UserId
            """,
            new
            {
                UserId = userId
            }
        );
    }
}
