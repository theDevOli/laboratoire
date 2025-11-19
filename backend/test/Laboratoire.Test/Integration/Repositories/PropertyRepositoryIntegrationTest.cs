using Dapper;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;
using Laboratoire.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Laboratoire.Test.Integration.Repositories;

public class PropertyRepositoryIntegrationTest
{
    private readonly PropertyRepository _repository;
    private readonly NpgsqlConnection _connection;

    public PropertyRepositoryIntegrationTest()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true)
            .Build();

        var dbContext = new Infrastructure.DbContext.DataContext(config);
        var connectionString = config.GetConnectionString("DefaultConnectionDev");

        _repository = new PropertyRepository(dbContext);
        _connection = new NpgsqlConnection(connectionString);
    }

    [Fact]
    public async Task GetAllPropertiesAsync_ShouldSucceed()
    {
        // Act
        var properties = await _repository.GetAllPropertiesAsync();

        // Assert
        Assert.NotEmpty(properties);
        Assert.IsAssignableFrom<IEnumerable<Property>>(properties);
    }

    [Fact]
    public async Task GetAllPropertiesByClientIdAsync_ShouldSucceed()
    {
        // Arrange
        var clientId = await _connection.QueryFirstOrDefaultAsync<Guid>
        (
            """
            SELECT 
                c.client_id
            FROM
                customers.client AS c
            INNER JOIN
                customers.property AS p
                ON p.client_id = c.client_id;
            """
        );
        // Act
        var properties = await _repository.GetAllPropertiesByClientIdAsync(clientId);

        // Assert
        Assert.NotEmpty(properties);
        Assert.IsAssignableFrom<IEnumerable<Property>>(properties);
    }

    [Fact]
    public async Task GetAllPropertiesDisplayAsync_ShouldSucceed()
    {
        // Act
        var properties = await _repository.GetAllPropertiesDisplayAsync<PropertyDtoDisplay>();

        // Assert
        Assert.NotEmpty(properties);
        Assert.IsAssignableFrom<IEnumerable<PropertyDtoDisplay>>(properties);
    }

    [Fact]
    public async Task GetPropertyByIdAsync_ShouldSucceed()
    {
        // Arrange
        var firstProperty = (await _repository.GetAllPropertiesDisplayAsync<Property>()).FirstOrDefault();

        // Act
        var result = await _repository.GetPropertyByIdAsync(firstProperty?.PropertyId);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<Property>(result);
    }

    [Fact]
    public async Task DoesPropertyExistAsync_ShouldSucceed()
    {
        // Arrange
        var firstProperty = (await _repository.GetAllPropertiesDisplayAsync<Property>()).FirstOrDefault();

        // Act
        var result = await _repository.DoesPropertyExistAsync(firstProperty!);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddPropertyAsync_ShouldSucceed()
    {
        // Arrange
        var firstProperty = (await _repository.GetAllPropertiesDisplayAsync<Property>()).FirstOrDefault();
        var newProperty = new Property()
        {
            PropertyId = default,
            ClientId = firstProperty?.ClientId,
            StateId = firstProperty?.StateId,
            PropertyName = "TEST",
            Registration = "TEST",
            City = "TEST",
            PostalCode = "4598800",
            Area = "12 TEST",
            Ccir = "TEST",
            ItrNirf = "TEST",
            Cei = "TEST"
        };

        // Act
        var result = await _repository.AddPropertyAsync(newProperty);
        var propertyId = await _connection.QueryFirstOrDefaultAsync<int>
        (
            """
            SELECT 
                property_id
            FROM
                customers.property
            WHERE
                area = @Area;
            """,
            new { Area = newProperty.Area }
        );

        // Assert
        Assert.True(result);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM customers.property WHERE property_id = @propertyId", new { propertyId });
    }

    [Fact]
    public async Task UpdatePropertyAsync_ShouldSucceed()
    {
        // Arrange
        var properties = (await _repository.GetAllPropertiesDisplayAsync<Property>()).ToArray();
        var newProperty = new Property()
        {
            PropertyId = default,
            ClientId = properties[0]?.ClientId,
            StateId = properties[0]?.StateId,
            PropertyName = "TEST",
            Registration = "TEST",
            City = "TEST",
            PostalCode = "4598800",
            Area = "12 TEST",
            Ccir = "TEST",
            ItrNirf = "TEST",
            Cei = "TEST"
        };
        await _repository.AddPropertyAsync(newProperty);
        var propertyId = await _connection.QueryFirstOrDefaultAsync<int>
        (
            """
            SELECT 
                property_id
            FROM
                customers.property
            WHERE
                area = @Area;
            """,
            new { Area = newProperty.Area }
        );

        var toUpdateProperty = new Property()
        {
            PropertyId = propertyId,
            ClientId = properties[1]?.ClientId,
            StateId = 1,
            PropertyName = "LEST",
            Registration = "LEST",
            City = "LEST",
            PostalCode = "0000000",
            Area = "12 LEST",
            Ccir = "LEST",
            ItrNirf = "LEST",
            Cei = "LEST"
        };

        // Act
        var result = await _repository.UpdatePropertyAsync(toUpdateProperty);
        var updatedProperty = await _repository.GetPropertyByIdAsync(propertyId);

        // Assert
        Assert.True(result);
        Assert.Equal(toUpdateProperty.PropertyId,updatedProperty?.PropertyId);
        Assert.Equal(toUpdateProperty.ClientId,updatedProperty?.ClientId);
        Assert.Equal(toUpdateProperty.StateId,updatedProperty?.StateId);
        Assert.Equal(toUpdateProperty.PropertyName,updatedProperty?.PropertyName);
        Assert.Equal(toUpdateProperty.Registration,updatedProperty?.Registration);
        Assert.Equal(toUpdateProperty.City,updatedProperty?.City);
        Assert.Equal(toUpdateProperty.PostalCode,updatedProperty?.PostalCode);
        Assert.Equal(toUpdateProperty.Area,updatedProperty?.Area);
        Assert.Equal(toUpdateProperty.Ccir,updatedProperty?.Ccir);
        Assert.Equal(toUpdateProperty.ItrNirf,updatedProperty?.ItrNirf);
        Assert.Equal(toUpdateProperty.Cei,updatedProperty?.Cei);

        // Tear down
        await _connection.ExecuteAsync("DELETE FROM customers.property WHERE property_id = @propertyId", new { propertyId });
    }
}
