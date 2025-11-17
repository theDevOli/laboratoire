using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class OfficeRepositoryIntegrationTest
{
    private readonly OfficeRepository _repository;
    private readonly NpgsqlConnection _connection;

    public OfficeRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new OfficeRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllOfficesAsync_ShouldSucceed()
    {
        // Act
        var offices = await _repository.GetAllOfficesAsync();

        // Assert
        Assert.NotEmpty(offices);
        Assert.IsAssignableFrom<IEnumerable<Office>>(offices);
    }

    [Fact]
    public async Task GetOfficeByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstOffice = (await _repository.GetAllOfficesAsync()).FirstOrDefault();

        // Act
        var office = await _repository.GetOfficeByIdAsync(firstOffice?.OfficeId);

        // Assert
        Assert.NotNull(office);
        Assert.IsType<Office>(office);
    }

    [Fact]
    public async Task DoesOfficeExistByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstOffice = (await _repository.GetAllOfficesAsync()).FirstOrDefault();

        // Act
        var doesExist = await _repository.DoesOfficeExistByIdAsync(firstOffice!);

        // Assert
        Assert.True(doesExist);
    }

    [Fact]
    public async Task DoesOfficeExistByCityAndNameAsync_ShouldSucceed()
    {
        // Arrange
        var firstOffice = (await _repository.GetAllOfficesAsync()).FirstOrDefault();

        // Act
        var doesExist = await _repository.DoesOfficeExistByCityAndNameAsync(firstOffice!);

        // Assert
        Assert.True(doesExist);
    }

    [Fact]
    public async Task AddOfficeAsync_ShouldSucceed()
    {
        // Arrange
        var newOffice = new Office()
        {
            OfficeId = default,
            OfficeName = "Test",
            OfficeEmail = "Test",
            City = "Test"
        };

        // Act
        var IsAdded = await _repository.AddOfficeAsync(newOffice);

        // Assert
        Assert.True(IsAdded);

        // Tear down

        await _connection.ExecuteAsync
        (
            """
            DELETE FROM customers.office
            WHERE
                office_name = @OfficeName
                AND office_email = @OfficeEmail
                AND city = @City;
            """,
            new
            {
                OfficeName = newOffice.OfficeName,
                OfficeEmail = newOffice.OfficeEmail,
                City = newOffice.City,
            }
        );
    }

    [Fact]
    public async Task UpdateOfficeAsync_ShouldSucceed()
    {
        // Arrange
        var newOffice = new Office()
        {
            OfficeId = default,
            OfficeName = "Test",
            OfficeEmail = "Test",
            City = "Test"
        };

        await _repository.AddOfficeAsync(newOffice);

        var officeId = await _connection.QuerySingleAsync<Guid>
        (
            """
            SELECT
                office_id
            FROM
                customers.office
            WHERE
                office_name = @OfficeName
                AND office_email = @OfficeEmail
                AND city = @City;
            """,
                new
                {
                    OfficeName = newOffice.OfficeName,
                    OfficeEmail = newOffice.OfficeEmail,
                    City = newOffice.City,
                }
        );

        var toUpdate = new Office()
        {
            OfficeId = officeId,
            OfficeName = "Lest",
            OfficeEmail = "Lest",
            City = "Lest"
        };

        // Act
        var isUpdated = await _repository.UpdateOfficeAsync(toUpdate);
        var updatedOffice = await _repository.GetOfficeByIdAsync(officeId);

        // Assert
        Assert.True(isUpdated);
        Assert.Equal(toUpdate.OfficeId,updatedOffice?.OfficeId);
        Assert.Equal(toUpdate.OfficeName,updatedOffice?.OfficeName);
        Assert.Equal(toUpdate.OfficeEmail,updatedOffice?.OfficeEmail);
        Assert.Equal(toUpdate.City,updatedOffice?.City);

        // Tear down
        await _connection.ExecuteAsync
        (
            """
            DELETE FROM customers.office
            WHERE
                office_name = @OfficeName
                AND office_email = @OfficeEmail
                AND city = @City;
            """,
            new
            {
                OfficeName = newOffice.OfficeName,
                OfficeEmail = newOffice.OfficeEmail,
                City = newOffice.City,
            }
        );
    }
}
