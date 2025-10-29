using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

public class OfficeRepository(DataContext dapper) : IOfficeRepository
{
    #region sql commands
    private readonly string _getAllSql =
    $"""
    SELECT
        office_id AS {nameof(Office.OfficeId)},
        office_name AS {nameof(Office.OfficeName)},
        office_email AS {nameof(Office.OfficeEmail)},
        city AS {nameof(Office.City)}
    FROM
        customers.office;
    """;
    private readonly string _getByIdSql =
    $"""
        SELECT
        office_id AS {nameof(Office.OfficeId)},
        office_name AS {nameof(Office.OfficeName)},
        office_email AS {nameof(Office.OfficeEmail)},
        city AS {nameof(Office.City)}
    FROM
        customers.office
    WHERE
        office_id = @OfficeIdParameter;
    """;

    private readonly string _addSql =
    $"""
    INSERT INTO customers.office
        (office_name,office_email,city)
    VALUES
        (@OfficeNameParameter,@OfficeEmailParameter,@CityParameter);
    """;
    private readonly string _updateSql =
    $"""
    UPDATE customers.office
    SET
        office_name = @OfficeNameParameter,
        office_email = @OfficeEmailParameter,
        city = @CityParameter
    WHERE
        office_id = @OfficeIdParameter;
    """;
    #endregion
    public async Task<bool> AddOfficeAsync(Office office)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@OfficeNameParameter", office.OfficeName, DbType.String);
        parameters.Add("@OfficeEmailParameter", office.OfficeEmail, DbType.String);
        parameters.Add("@CityParameter", office.City, DbType.String);

        return await dapper.ExecuteSqlAsync(_addSql, parameters);
    }

    public async Task<bool> DoesOfficeExistAsync(Office office)
    => await GetOfficeByIdAsync(office.OfficeId) is not null;

    public async Task<IEnumerable<Office>> GetAllOfficesAsync()
    => await dapper.LoadDataAsync<Office>(_getAllSql);

    public async Task<Office?> GetOfficeByIdAsync(Guid? officeId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@OfficeIdParameter", officeId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Office>(_getByIdSql, parameters);
    }

    public async Task<bool> UpdateOfficeAsync(Office office)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@OfficeNameParameter", office.OfficeName, DbType.String);
        parameters.Add("@OfficeEmailParameter", office.OfficeEmail, DbType.String);
        parameters.Add("@CityParameter", office.City, DbType.String);
        parameters.Add("@OfficeIdParameter", office.OfficeId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_updateSql,parameters);
    }
}
