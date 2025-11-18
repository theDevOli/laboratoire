using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class PartnerRepositoryIntegrationTest
{
    private readonly PartnerRepository _repository;
    private readonly UserRepository _userRepository;
    private readonly NpgsqlConnection _connection;

    public PartnerRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new PartnerRepository(dbContext);
        _userRepository = new UserRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllPartnersAsync_ShouldSucceed()
    {
        // Act
        var partners = await _repository.GetAllPartnersAsync();

        //Assert
        Assert.NotEmpty(partners);
        Assert.IsAssignableFrom<IEnumerable<Partner>>(partners);
    }

    [Fact]
    public async Task GetActivePartnersAsync_ShouldSucceed()
    {
        // Arrange
        var users = await _userRepository.GetAllUsersAsync();

        // Act
        var partners = await _repository.GetActivePartnersAsync();
        var partnerRoleId = users.SingleOrDefault(x => x.UserId == partners.FirstOrDefault()!.UserId);
        var activePartners = users.Where(u => u.IsActive == true && u.RoleId == partnerRoleId!.RoleId);

        //Assert
        Assert.NotEmpty(partners);
        Assert.IsAssignableFrom<IEnumerable<Partner>>(partners);
        Assert.Equal(partners.Count(), activePartners.Count());
    }

    [Fact]
    public async Task GetPartnerByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstPartner = (await _repository.GetAllPartnersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetPartnerByIdAsync(firstPartner?.PartnerId);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Partner>(result);
    }

    [Fact]
    public async Task GetPartnerByOfficeAndNameAsync_ShouldSucceed()
    {
        // Arrange
        var firstPartner = (await _repository.GetAllPartnersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.GetPartnerByOfficeAndNameAsync(firstPartner!);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<Partner>(result);
    }

    [Fact]
    public async Task DoesPartnerExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstPartner = (await _repository.GetAllPartnersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesPartnerExistByIdAsync(firstPartner!);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoesPartnerExistByOfficeAndNameAsync_ShouldSucceed()
    {
        // Arrange
        var firstPartner = (await _repository.GetAllPartnersAsync()).FirstOrDefault();

        // Act
        var result = await _repository.DoesPartnerExistByOfficeAndNameAsync(firstPartner!);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdatePartnerAsync_ShouldSucceed()
    {
        // Arrange
        var users = await _userRepository.GetAllUsersAsync();

        // Act
        var partners = (await _repository.GetAllPartnersAsync()).ToArray();
        var partnerRoleId = users.SingleOrDefault(x => x.UserId == partners.FirstOrDefault()!.UserId)?.RoleId;

        var newPartner = new Partner()
        {
            PartnerId = default,
            OfficeId = partners[0].OfficeId,
            UserId = default,
            PartnerName = "Test",
            PartnerPhone = "7999884513"
        };
        var neuUser = new User()
        {
            UserId = default,
            RoleId = partnerRoleId,
            Username = "Test-username",
            IsActive = true
        };

        await _userRepository.AddUserAndPartnerAsync(neuUser, newPartner);

        var partnerDB = await _repository.GetPartnerByOfficeAndNameAsync(newPartner);

        var toUpdatePartner = new Partner()
        {
            PartnerId = partnerDB?.PartnerId,
            OfficeId = partners[1].OfficeId,
            UserId = partnerDB?.UserId,
            PartnerName = "Lest",
            PartnerPhone = "0009884000"
        };

        // Act
        var isUpdated = await _repository.UpdatePartnerAsync(toUpdatePartner);
        var updatedPartner = await _repository.GetPartnerByIdAsync(partnerDB?.PartnerId);

        //Assert
        Assert.True(isUpdated);
        Assert.Equal(toUpdatePartner.PartnerId, updatedPartner?.PartnerId);
        Assert.Equal(toUpdatePartner.OfficeId, updatedPartner?.OfficeId);
        Assert.Equal(toUpdatePartner.UserId, updatedPartner?.UserId);
        Assert.Equal(toUpdatePartner.PartnerName, updatedPartner?.PartnerName);
        Assert.Equal(toUpdatePartner.PartnerPhone, updatedPartner?.PartnerPhone);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM customers.partner WHERE user_id = @UserId", new { UserId = partnerDB?.UserId, });
        await _connection.ExecuteAsync("""DELETE FROM users."user" WHERE user_id = @UserId""", new { UserId = partnerDB?.UserId, });
    }
}
