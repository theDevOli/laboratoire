using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class RoleRepository(DataContext dapper) : IRoleRepository
{
    #region SQL queries
    private readonly string _getAllSql =
    $"""
    SELECT
        role_id AS {nameof(Role.RoleId)},
        role_name AS {nameof(Role.RoleName)}
    FROM
        users.role;
    """;
    private readonly string _getByIdSql =
    $"""
    SELECT
        role_id AS {nameof(Role.RoleId)},
        role_name AS {nameof(Role.RoleName)}
    FROM
        users.role
    WHERE
        role_id = @RoleIdParameter;
    """;
    private readonly string _getByNameSql =
    $"""
    SELECT
        role_id AS {nameof(Role.RoleId)},
        role_name AS {nameof(Role.RoleName)}
    FROM
        users.role
    WHERE 
        role_name = @RoleNameParameter;
    """;
    private readonly string _getRoleByUserIdSql =
 $"""
    SELECT 
        r.role_name AS {nameof(Role.RoleName)}
    FROM
        users.user AS u
    INNER JOIN
        users.role AS r 
        ON u.role_id = r.role_id
    WHERE
        user_id = @UserIdParameter;
    """;
    private readonly string _addSql =
    $"""
    INSERT INTO users.role
        (role_name)
    VALUES
        (@RoleNameParameter);
    """;
    private readonly string _updateSql =
    $"""
    UPDATE users.role
    SET 
        role_name = @RoleNameParameter
    WHERE 
        role_id = @RoleIdParameter;
    """;
    #endregion
    public async Task<bool> AddRoleAsync(Role role)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleNameParameter", role.RoleName, DbType.String);

        return await dapper.ExecuteSqlAsync(_addSql, parameters);
    }

    public async Task<bool> DoesRoleExistByNameAsync(Role role)
    => await GetRoleByNameAsync(role.RoleName) != null;

    public async Task<bool> DoesRoleExistByIdAsync(Role role)
    => await GetRoleByIdAsync(role.RoleId) != null;

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    => await dapper.LoadDataAsync<Role>(_getAllSql);

    public async Task<Role?> GetRoleByIdAsync(int? id)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", id, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Role>(_getByIdSql, parameters);
    }

    public async Task<Role?> GetRoleByNameAsync(string? roleName)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleNameParameter", roleName, DbType.String);

        return await dapper.LoadDataSingleAsync<Role>(_getByNameSql, parameters);
    }

    public async Task<bool> UpdateRoleAsync(Role role)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", role.RoleId, DbType.Int32);
        parameters.Add("@RoleNameParameter", role.RoleName, DbType.String);

        return await dapper.ExecuteSqlAsync(_updateSql, parameters);
    }

    public async Task<string?> GetRoleNameByUserIdAsync(Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserIdParameter", userId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<string>(_getRoleByUserIdSql, parameters);
    }
}
