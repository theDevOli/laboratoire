using Dapper;
using Laboratoire.Application.Services.OfficeServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class OfficeIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public OfficeIntegrationTest()
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
    public async Task UpdateOffice_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new OfficeRepository(_dbContext);

        var service = new OfficeUpdatableService(repository, NullLogger<OfficeUpdatableService>.Instance);

        var officeId = await connection.ExecuteScalarAsync<Guid>
        (
            $"""
            INSERT INTO customers.office
                (office_name,office_email,city)
            VALUES
                (@OfficeName,@OfficeEmail,@City)
            RETURNING office_id;
            """,
            new
            {
                OfficeName = "Test",
                OfficeEmail = "test@email.com",
                City = "Test"
            }
        );

        var officeToUpdate = new Office() { OfficeId = officeId,OfficeName="Updated", OfficeEmail = "updated@email.com", City = "Updated" };

        // Act
        var result = await service.UpdateOfficeAsync(officeToUpdate);
        var updatedOffice = await repository.GetOfficeByIdAsync(officeId);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Null(result.Message);
        Assert.Equal(officeToUpdate.OfficeName, updatedOffice!.OfficeName);
        Assert.Equal(officeToUpdate.OfficeEmail, updatedOffice!.OfficeEmail);
        Assert.Equal(officeToUpdate.City, updatedOffice!.City);

        // Tear up
        await connection.ExecuteAsync
        (
            """
            DELETE FROM customers.office
            WHERE office_id = @OfficeId;
            """,
            new
            {
                OfficeId = officeId
            }
        );
    }
}
