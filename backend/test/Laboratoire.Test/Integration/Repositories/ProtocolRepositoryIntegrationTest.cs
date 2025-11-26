using Dapper;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class ProtocolRepositoryIntegrationTest
{
    private readonly ProtocolRepository _repository;
    private readonly ClientRepository _clientRepository;
    private readonly UserRepository _userRepository;
    private readonly PartnerRepository _partnerRepository;
    private readonly CatalogRepository _catalogRepository;
    private readonly CashFlowRepository _cashFlowRepository;
    private readonly NpgsqlConnection _connection;

    public ProtocolRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new ProtocolRepository(dbContext);
        _clientRepository = new ClientRepository(dbContext);
        _userRepository = new UserRepository(dbContext);
        _partnerRepository = new PartnerRepository(dbContext);
        _catalogRepository = new CatalogRepository(dbContext);
        _cashFlowRepository = new CashFlowRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllProtocolsAsync_ShouldSucceed()
    {
        // Act
        var protocols = await _repository.GetAllProtocolsAsync();

        // Assert
        Assert.NotEmpty(protocols);
        Assert.IsAssignableFrom<IEnumerable<Protocol>>(protocols);
    }

    [Fact]
    public async Task GetDisplayProtocolsAsync_ShouldSucceed()
    {
        // Arrange
        int year = 2025;
        // Act
        var protocols = await _repository.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year);

        // Assert
        Assert.NotEmpty(protocols);
        Assert.IsAssignableFrom<IEnumerable<ProtocolDtoDisplayDb>>(protocols);
    }

    [Fact]
    public async Task GetProtocolYearsAsync_ShouldSucceed()
    {
        // Act
        var years = await _repository.GetProtocolYearsAsync<ProtocolDtoYears>();

        // Assert
        Assert.NotEmpty(years);
        Assert.IsAssignableFrom<IEnumerable<ProtocolDtoYears>>(years);
    }

    [Fact]
    public async Task GetProtocolByProtocolIdAsync_ShouldSucceed()
    {
        // Arrange
        var protocolId = (await _repository.GetAllProtocolsAsync()).FirstOrDefault()?.ProtocolId;
        // Act
        var protocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.NotNull(protocol);
        Assert.IsType<Protocol>(protocol);
    }

    [Fact]
    public async Task DoesProtocolExistByReportIdAsync_ShouldSucceed()
    {
        // Arrange
        var reportId = (await _repository.GetAllProtocolsAsync()).FirstOrDefault()?.ReportId;
        // Act
        var protocol = await _repository.DoesProtocolExistByReportIdAsync(reportId);

        // Assert
        Assert.True(protocol);
    }

    [Fact]
    public async Task GetProtocolByReportIdAsync_ShouldSucceed()
    {
        // Arrange
        var reportId = (await _repository.GetAllProtocolsAsync()).FirstOrDefault()?.ReportId;
        // Act
        var protocol = await _repository.GetProtocolByReportIdAsync(reportId);

        // Assert
        Assert.NotNull(protocol);
        Assert.IsType<Protocol>(protocol);
    }

    [Fact]
    public async Task GetUniqueProtocolAsync_ShouldSucceed()
    {
        // Arrange
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();
        // Act
        var protocol = await _repository.GetUniqueProtocolAsync(firstProtocol!);

        // Assert
        Assert.NotNull(protocol);
        Assert.IsType<Protocol>(protocol);
    }

    [Fact]
    public async Task DoesProtocolExistByUniqueAsync_ShouldSucceed()
    {
        // Arrange
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();
        // Act
        var protocol = await _repository.DoesProtocolExistByUniqueAsync(firstProtocol!);

        // Assert
        Assert.True(protocol);
    }

    [Fact]
    public async Task DoesProtocolExistByProtocolIdAsync_WhenProtocolIsParameter_ShouldSucceed()
    {
        // Arrange
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();
        // Act
        var protocol = await _repository.DoesProtocolExistByProtocolIdAsync(firstProtocol!);

        // Assert
        Assert.True(protocol);
    }

    [Fact]
    public async Task DoesProtocolExistByProtocolIdAsync_WhenProtocolIdIsParameter_ShouldSucceed()
    {
        // Arrange
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();
        // Act
        var protocol = await _repository.DoesProtocolExistByProtocolIdAsync(firstProtocol!.ProtocolId);

        // Assert
        Assert.True(protocol);
    }

    [Fact]
    public async Task IsProtocolDoubled_ShouldSucceed()
    {
        // Arrange
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();
        // Act
        var protocol = await _repository.IsProtocolDoubled(firstProtocol!.ProtocolId);

        // Assert
        Assert.False(protocol);
    }

    [Fact]
    public async Task AddProtocolAsync_ShouldSucceed()
    {
        // Arrange
        var now = DateTime.Now;
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();
        _connection.Open();

        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = firstProtocol?.ClientId,
            PropertyId = firstProtocol?.PropertyId,
            PartnerId = default,
            CatalogId = firstProtocol?.CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = true,
        };
        // Act
        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        // Assert
        Assert.NotNull(protocolId);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM document.protocol WHERE protocol_id = @protocolId;", new { protocolId });
        _connection.Close();
    }

    [Fact]
    public async Task DeleteProtocolAsync_ShouldSucceed()
    {
        // Arrange
        var now = DateTime.Now;
        var firstProtocol = (await _repository.GetAllProtocolsAsync()).FirstOrDefault();


        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = firstProtocol?.ClientId,
            PropertyId = firstProtocol?.PropertyId,
            PartnerId = default,
            CatalogId = firstProtocol?.CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = true,
        };
        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        // Act
        var result = await _repository.DeleteProtocolAsync(protocolId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SaveProtocolSpotAsync_ShouldSucceed()
    {
        // Arrange
        var taxId = "00011122289";
        var quantity = 5;
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
            ClientTaxId = taxId,
            ClientEmail = "test@email.com",
            ClientPhone = "Test",
        };

        await _clientRepository.AddClientAsync(newClient, userId);

        var client = await _clientRepository.GetByTaxIdAsync(taxId);


        // Act
        var result = await _repository.SaveProtocolSpotAsync(quantity, client?.ClientId, 1);

        var protocols = (await _repository.GetAllProtocolsAsync()).Where(p => p.ClientId == client?.ClientId);

        // Assert
        Assert.True(result);
        Assert.Equal(quantity, protocols.Count());

        // Tear down
        var protocolIds = protocols.Select(p => p.ProtocolId).ToArray();

        await _connection.OpenAsync();

        var transaction = await _connection.BeginTransactionAsync();

        await _connection.ExecuteAsync("DELETE FROM document.protocol WHERE protocol_id = ANY(@protocolIds);", new { protocolIds });

        await _connection.ExecuteAsync
        (
            """
            DELETE FROM customers.client    
            WHERE client_id = @ClientId;
            """,
            new
            {
                ClientId = client?.ClientId
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

        transaction.Commit();

        await _connection.CloseAsync();
    }

    [Fact]
    public async Task UpdateProtocolAsync_ShouldSucceed()
    {
        // Arrange
        var protocols = (await _repository.GetAllProtocolsAsync()).ToArray();
        var partners = (await _partnerRepository.GetAllPartnersAsync()).ToArray();
        var now = DateTime.Now;
        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = partners[0].PartnerId,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        var toUpdateProtocol = new Protocol()
        {
            ProtocolId = protocolId,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[1].ClientId,
            PropertyId = protocols[1].PropertyId,
            PartnerId = partners[1].PartnerId,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now.AddDays(3),
            ReportDate = now.AddDays(6),
            IsCollectedByClient = true,
        };

        //    Act
        var result = await _repository.UpdateProtocolAsync(toUpdateProtocol);
        var updatedProtocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateProtocol.ProtocolId, updatedProtocol?.ProtocolId);
        Assert.Equal(toUpdateProtocol.ClientId, updatedProtocol?.ClientId);
        Assert.Equal(toUpdateProtocol.PropertyId, updatedProtocol?.PropertyId);
        Assert.Equal(toUpdateProtocol.PartnerId, updatedProtocol?.PartnerId);
        Assert.Equal(toUpdateProtocol.EntryDate.Value!.Date, updatedProtocol?.EntryDate!.Value.Date);
        Assert.Equal(toUpdateProtocol.ReportDate.Value.Date, updatedProtocol?.ReportDate!.Value.Date);
        Assert.Equal(toUpdateProtocol.IsCollectedByClient, updatedProtocol?.IsCollectedByClient);

        // Tear down
        await _repository.DeleteProtocolAsync(protocolId);
    }

    [Fact]
    public async Task UpdateCatalogAsync_ShouldSucceed()
    {
        // Arrange
        var protocols = (await _repository.GetAllProtocolsAsync()).ToArray();
        var catalogs = (await _catalogRepository.GetAllCatalogsAsync()).ToArray();
        var now = DateTime.Now;
        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = catalogs[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        var toUpdateProtocol = new Protocol()
        {
            ProtocolId = protocolId,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = catalogs[1].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        //    Act
        var result = await _repository.UpdateCatalogAsync(toUpdateProtocol);
        var updatedProtocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateProtocol.CatalogId, updatedProtocol?.CatalogId);

        // Tear down
        await _repository.DeleteProtocolAsync(protocolId);
    }

    [Fact]
    public async Task UpdateCashFlowIdAsync_ShouldSucceed()
    {
        // Arrange
        var protocols = (await _repository.GetAllProtocolsAsync()).ToArray();
        var cashFlows = (await _cashFlowRepository.GetAllCashFlowAsync()).ToArray();
        var now = DateTime.Now;
        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = cashFlows[0].CashFlowId,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        var toUpdateProtocol = new Protocol()
        {
            ProtocolId = protocolId,
            CashFlowId = cashFlows[1].CashFlowId,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        //    Act
        var result = await _repository.UpdateCatalogAsync(toUpdateProtocol);
        var updatedProtocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateProtocol.CatalogId, updatedProtocol?.CatalogId);

        // Tear down
        await _repository.DeleteProtocolAsync(protocolId);
    }

    [Fact]
    public async Task PatchReportIdAsync_ShouldSucceed()
    {
        // Arrange
        var protocols = (await _repository.GetAllProtocolsAsync()).ToArray();

        var now = DateTime.Now;
        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        var newReportPatch = new ReportPatch()
        {
            ProtocolId = protocolId,
            ReportId = protocols[0].ReportId,
        };

        //    Act
        var result = await _repository.PatchReportIdAsync(newReportPatch);
        var updatedProtocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.True(result);
        Assert.Equal(newReportPatch.ReportId, updatedProtocol?.ReportId);

        // Tear down
        await _repository.DeleteProtocolAsync(protocolId);
    }

    [Fact]
    public async Task PatchReportAsync_ShouldSucceed()
    {
        // Arrange
        var protocols = (await _repository.GetAllProtocolsAsync()).ToArray();

        var now = DateTime.Now;
        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        var newReportPatch = new ReportPatch()
        {
            ProtocolId = protocolId,
            ReportId = protocols[0].ReportId,
        };

        //    Act
        var result = await _repository.PatchReportIdAsync(newReportPatch);
        var updatedProtocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);

        // Assert
        Assert.True(result);
        Assert.Equal(newReportPatch.ReportId, updatedProtocol?.ReportId);

        // Tear down
        await _repository.DeleteProtocolAsync(protocolId);
    }

    [Fact]
    public async Task PatchCashFlowIdWithDescriptionAsync_ShouldSucceed()
    {
        // Arrange
        var description = "Test Description for CashFlow";
        var protocols = (await _repository.GetAllProtocolsAsync()).ToArray();
        var newCashFlow = new CashFlow()
        {
            CashFlowId = default,
            TransactionId = 1,
            Description = default,
            PartnerId = default,
            TotalPaid = 10m,
            PaymentDate = DateTime.Now,
        };

        var cashFlowId = await _cashFlowRepository.AddCashFlowAsync(newCashFlow);

        var now = DateTime.Now;
        var newProtocol = new Protocol()
        {
            ProtocolId = default,
            CashFlowId = default,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        var protocolId = await _repository.AddProtocolAsync(newProtocol);

        var toUpdateProtocol = new Protocol()
        {
            ProtocolId = protocolId,
            CashFlowId = cashFlowId,
            ReportId = default,
            ClientId = protocols[0].ClientId,
            PropertyId = protocols[0].PropertyId,
            PartnerId = default,
            CatalogId = protocols[0].CatalogId,
            EntryDate = now,
            ReportDate = now.AddDays(3),
            IsCollectedByClient = false,
        };

        //    Act
        var result = await _repository.PatchCashFlowIdWithDescriptionAsync(toUpdateProtocol, description);
        var updatedProtocol = await _repository.GetProtocolByProtocolIdAsync(protocolId);
        var updatedCashFlow = await _cashFlowRepository.GetCashFlowByIdAsync(cashFlowId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateProtocol.CashFlowId, updatedProtocol?.CashFlowId);
        Assert.Equal(updatedCashFlow?.Description, description);

        // Tear down
        await _repository.DeleteProtocolAsync(protocolId);
    }
}
