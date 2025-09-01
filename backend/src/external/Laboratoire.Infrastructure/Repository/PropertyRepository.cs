using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class PropertyRepository(DataContext dapper) : IPropertyRepository
{
    #region SQL queries
    private readonly string _getAllPropertiesSql =
    $"""
    SELECT
        property_id AS {nameof(Property.PropertyId)},
        client_id AS {nameof(Property.ClientId)},
        state_id AS {nameof(Property.StateId)},
        property_name AS {nameof(Property.PropertyName)},
        city AS {nameof(Property.City)},
        registration AS {nameof(Property.Registration)},
        postal_code AS {nameof(Property.PostalCode)},
        area AS {nameof(Property.Area)},
        ccir AS {nameof(Property.Ccir)},
        itr_nirf AS {nameof(Property.ItrNirf)}
    FROM
        customers.property;
    """;
    private readonly string _getAllPropertiesByClientIdSql =
    $"""
    SELECT
        property_id AS {nameof(Property.PropertyId)},
        client_id AS {nameof(Property.ClientId)},
        state_id AS {nameof(Property.StateId)},
        property_name AS {nameof(Property.PropertyName)},
        city AS {nameof(Property.City)},
        registration AS {nameof(Property.Registration)},
        postal_code AS {nameof(Property.PostalCode)},
        area AS {nameof(Property.Area)},
        ccir AS {nameof(Property.Ccir)},
        itr_nirf AS {nameof(Property.ItrNirf)}
    FROM 
        customers.property
    WHERE 
        client_id = @ClientIdParameter
    ORDER BY 
        state_id, city, property_name;
    """;
    private readonly string _getAllPropertiesToDisplaySql =
    $"""
    SELECT 
        p.property_id AS {nameof(Property.PropertyId)},
        p.property_name AS {nameof(Property.PropertyName)},
        p.city AS {nameof(Property.City)},
        p.postal_code AS {nameof(Property.PostalCode)},
        p.area AS {nameof(Property.Area)},
        p.ccir AS {nameof(Property.Ccir)},
        p.itr_nirf AS {nameof(Property.ItrNirf)},
        p.registration AS {nameof(Property.Registration)},

        c.client_id AS {nameof(Client.ClientId)},
        c.client_name AS {nameof(Client.ClientName)},
        c.client_tax_id AS {nameof(Client.ClientTaxId)},

        s.state_id AS {nameof(State.StateId)},
        s.state_code AS {nameof(State.StateCode)}
    FROM 
        customers.property AS p
    INNER JOIN
        customers.client AS c
        USING(client_id)
    INNER JOIN
        utils.state AS s
        USING(state_id)
    ORDER BY
        c.client_name,p.property_name;
    """;
    private readonly string _getPropertyByIdSql =
    $"""
    SELECT
        property_id AS {nameof(Property.PropertyId)},
        client_id AS {nameof(Property.ClientId)},
        state_id AS {nameof(Property.StateId)},
        property_name AS {nameof(Property.PropertyName)},
        city AS {nameof(Property.City)},
        registration AS {nameof(Property.Registration)},
        postal_code AS {nameof(Property.PostalCode)},
        area AS {nameof(Property.Area)},
        ccir AS {nameof(Property.Ccir)},
        itr_nirf AS {nameof(Property.ItrNirf)}
    FROM
        customers.property
    WHERE
        property_id = @PropertyIdParameter;
    """;
    private readonly string _addPropertySql =
    $"""
    INSERT INTO customers.property (
        client_id,
        state_id,
        property_name,
        city,
        postal_code,
        registration,
        area,
        ccir,
        itr_nirf
    )
    VALUES 
    (
        @ClientIdParameter,
        @StateIdParameter,
        @PropertyNameParameter,
        @CityParameter,
        @PostalCodeParameter,
        @RegistrationParameter,
        @AreaParameter,
        @CcirParameter,
        @ItrNirfParameter
    );
    """;
    private readonly string _updateProperty =
    $"""
    UPDATE customers.property
    SET
        client_id = @ClientIdParameter,
        state_id = @StateIdParameter,
        property_name = @PropertyNameParameter,
        city = @CityParameter,
        postal_code = @PostalCodeParameter,
        area = @AreaParameter,
        registration = @RegistrationParameter,
        ccir = @CcirParameter,
        itr_nirf = @ItrNirfParameter
    WHERE
        property_id = @PropertyIdParameter;
    """;
    #endregion
    public async Task<bool> AddPropertyAsync(Property property)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientIdParameter", property.ClientId, DbType.Guid);
        parameters.Add("@StateIdParameter", property.StateId, DbType.Int32);
        parameters.Add("@PropertyNameParameter", property.PropertyName, DbType.String);
        parameters.Add("@CityParameter", property.City, DbType.String);
        parameters.Add("@PostalCodeParameter", property.PostalCode, DbType.String);
        parameters.Add("@RegistrationParameter", property.Registration, DbType.String);
        parameters.Add("@AreaParameter", property.Area, DbType.String);
        parameters.Add("@CcirParameter", property.Ccir, DbType.String);
        parameters.Add("@ItrNirfParameter", property.ItrNirf, DbType.String);

        return await dapper.ExecuteSqlAsync(_addPropertySql, parameters);
    }
    public async Task<bool> DoesPropertyExistAsync(Property property)
    => await GetPropertyByIdAsync(property.PropertyId) is not null;
    public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
    => await dapper.LoadDataAsync<Property>(_getAllPropertiesSql);
    public async Task<IEnumerable<Property>> GetAllPropertiesByClientIdAsync(Guid? clientId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientIdParameter", clientId, DbType.Guid);

        return await dapper.LoadDataAsync<Property>(_getAllPropertiesByClientIdSql, parameters);

    }
    public async Task<IEnumerable<T>> GetAllPropertiesDisplayAsync<T>()
    => await dapper.LoadDataAsync<T>(_getAllPropertiesToDisplaySql);
    public async Task<Property?> GetPropertyByIdAsync(int? propertyId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PropertyIdParameter", propertyId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Property>(_getPropertyByIdSql, parameters);
    }
    public async Task<bool> UpdatePropertyAsync(Property property)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@ClientIdParameter", property.ClientId, DbType.Guid);
        parameters.Add("@StateIdParameter", property.StateId, DbType.Int32);
        parameters.Add("@PropertyNameParameter", property.PropertyName, DbType.String);
        parameters.Add("@CityParameter", property.City, DbType.String);
        parameters.Add("@PostalCodeParameter", property.PostalCode, DbType.String);
        parameters.Add("@RegistrationParameter", property.Registration, DbType.String);
        parameters.Add("@AreaParameter", property.Area, DbType.String);
        parameters.Add("@CcirParameter", property.Ccir, DbType.String);
        parameters.Add("@ItrNirfParameter", property.ItrNirf, DbType.String);
        parameters.Add("@PropertyIdParameter", property.PropertyId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updateProperty, parameters);
    }
}
