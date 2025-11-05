using Dapper;
using Laboratoire.Application.Services.PartnerServices;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace Laboratoire.Test.Integration.Services;

public class PartnerIntegrationTest
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;
    private readonly DataContext _dbContext;

    public PartnerIntegrationTest()
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
    public async Task UpdatePartner_ShouldSucceed()
    {
        // Arrange
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var repository = new PartnerRepository(_dbContext);

        var service = new PartnerUpdatableService(repository, NullLogger<PartnerUpdatableService>.Instance);

        var office = await connection.QueryFirstOrDefaultAsync<Office>(
            """
            SELECT 
                *
            FROM customers.office;
            """
         );

        Guid partnerId = await connection.ExecuteScalarAsync<Guid>
        (
            """
            WITH new_user AS(
                INSERT INTO users."user" (role_id, username, is_active)
                VALUES (@roleId, @username, @isActive)
                RETURNING user_id
            )
            INSERT INTO customers.partner(
                partner_name,
                office_id,
                partner_phone,
                user_id
            )
            SELECT
                @PartnerName,
                @OfficeId,
                @PartnerPhone,
                user_id
            FROM
                new_user
            RETURNING partner_id;
            """,
             new
             {
                 roleId = 4,
                 username = "Test",
                 isActive = true,
                 PartnerName = "Test",
                 OfficeId = office?.OfficeId,
                 PartnerPhone = "99000000000"
             }
        );

        Partner toUpdate = new()
        {
            PartnerId = partnerId,
            PartnerName = "Updated",
            OfficeId = office?.OfficeId,
            PartnerPhone = "99000000000"
        };

        // Act
        var response = await service.UpdatePartnerAsync(toUpdate);

        var updated = await repository.GetPartnerByIdAsync(partnerId);

        // Assert
        Assert.False(response.IsNotSuccess());
        Assert.NotNull(updated);
        Assert.Equal(updated.PartnerName, toUpdate.PartnerName);

        await connection.ExecuteAsync("DELETE FROM customers.partner WHERE partner_id = @partnerId", new { partnerId = updated.PartnerId });
        await connection.ExecuteAsync("DELETE FROM users.\"user\" WHERE user_id = @userId", new { userId = updated.UserId });
    }

}
