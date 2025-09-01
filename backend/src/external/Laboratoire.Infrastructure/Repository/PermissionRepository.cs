using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

public sealed class PermissionRepository(DataContext dapper): IPermissionRepository
{
    #region SQL queries
    private readonly string _getAllPermissionsSql =
    $"""
    SELECT  
        p.permission_id AS {nameof(Permission.PermissionId)},
        p.protocol AS {nameof(Permission.Protocol)},
        p.client AS {nameof(Permission.Client)},
        p.property AS {nameof(Permission.Property)},
        p.cash_flow AS {nameof(Permission.CashFlow)},
        p.partner AS {nameof(Permission.Partner)},
        p.users AS {nameof(Permission.Users)},
        p.chemical AS {nameof(Permission.Chemical)},

        r.role_id AS {nameof(Role.RoleId)},
        r.role_name AS {nameof(Role.RoleName)}
    FROM
        users.permission AS p
    INNER JOIN 
        users.role AS r
        ON p.role_id = r.role_id
    ORDER BY 
        r.role_name;
    """;
    private readonly string _getPermissionByRoleIdSql =
    $"""
    SELECT  
        permission_id AS {nameof(Permission.PermissionId)},
        role_id AS {nameof(Permission.RoleId)},
        protocol AS {nameof(Permission.Protocol)},
        client AS {nameof(Permission.Client)},
        property AS {nameof(Permission.Property)},
        cash_flow AS {nameof(Permission.CashFlow)},
        partner AS {nameof(Permission.Partner)},
        users AS {nameof(Permission.Users)},
        chemical AS {nameof(Permission.Chemical)}
    FROM 
        users.permission
    WHERE 
        role_id = @RoleIdParameter;
    """;
    private readonly string _getPermissionByPermissionIdSql =
    $"""
    SELECT  
        permission_id AS {nameof(Permission.PermissionId)},
        role_id AS {nameof(Permission.RoleId)},
        protocol AS {nameof(Permission.Protocol)},
        client AS {nameof(Permission.Client)},
        property AS {nameof(Permission.Property)},
        cash_flow AS {nameof(Permission.CashFlow)},
        partner AS {nameof(Permission.Partner)},
        users AS {nameof(Permission.Users)},
        chemical AS {nameof(Permission.Chemical)}
    FROM 
        users.permission
    WHERE
        permission_id = @PermissionIdParameter;
    """;
    private readonly string _addPermissionSql =
    $"""
    INSERT INTO users.permission(
        role_id,
        protocol,
        client,
        property,
        cash_flow,
        partner,
        users,
        chemical
    )
    VALUES
    (
        @RoleIdParameter,
        @ProtocolParameter,
        @ClientParameter,
        @PropertyParameter,
        @CashFlowParameter,
        @PartnerParameter,
        @UsersParameter,
        @ChemicalParameter
    );
    """;
    private readonly string _updatePermissionSql =
    $"""
    UPDATE users.permission
    SET
        role_id = @RoleIdParameter,
        protocol = @ProtocolParameter,
        client = @ClientParameter,
        property = @PropertyParameter,
        cash_flow = @CashFlowParameter,
        partner = @PartnerParameter,
        users = @UsersParameter,
        chemical = @ChemicalParameter
    WHERE 
        permission_id = @PermissionIdParameter;
    """;
   #endregion
    public async Task<bool> AddPermissionAsync(Permission permission)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", permission.RoleId, DbType.Int32);
        parameters.Add("@ProtocolParameter", permission.Protocol, DbType.Boolean);
        parameters.Add("@ClientParameter", permission.Client, DbType.Boolean);
        parameters.Add("@PropertyParameter", permission.Property, DbType.Boolean);
        parameters.Add("@CashFlowParameter", permission.CashFlow, DbType.Boolean);
        parameters.Add("@PartnerParameter", permission.Partner, DbType.Boolean);
        parameters.Add("@UsersParameter", permission.Users, DbType.Boolean);
        parameters.Add("@ChemicalParameter", permission.Chemical, DbType.Boolean);

        return await dapper.ExecuteSqlAsync(_addPermissionSql, parameters);
    }
    public async Task<bool> DoesPermissionExistByPermissionIdAsync(Permission permission)
    => await GetPermissionByPermissionIdAsync(permission.PermissionId) is not null;
    public async Task<bool> DoesPermissionExistByRoleIdAsync(Permission permission)
    => await GetPermissionByRoleIdAsync(permission.RoleId) is not null;
    public async Task<IEnumerable<DisplayPermission>> GetAllPermissionsAsync()
    => await dapper.LoadDataAsync<DisplayPermission>(_getAllPermissionsSql);
    public async Task<Permission?> GetPermissionByPermissionIdAsync(int? permissionId)
    {

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PermissionIdParameter", permissionId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Permission>(_getPermissionByPermissionIdSql, parameters);
    }
    public async Task<Permission?> GetPermissionByRoleIdAsync(int? roleId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", roleId, DbType.Int32);

        return await dapper.LoadDataSingleAsync<Permission>(_getPermissionByRoleIdSql, parameters);
    }
    public async Task<bool> UpdatePermissionAsync(Permission permission)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", permission.RoleId, DbType.Int32);
        parameters.Add("@ProtocolParameter", permission.Protocol, DbType.Boolean);
        parameters.Add("@ClientParameter", permission.Client, DbType.Boolean);
        parameters.Add("@PropertyParameter", permission.Property, DbType.Boolean);
        parameters.Add("@CashFlowParameter", permission.CashFlow, DbType.Boolean);
        parameters.Add("@PartnerParameter", permission.Partner, DbType.Boolean);
        parameters.Add("@UsersParameter", permission.Users, DbType.Boolean);
        parameters.Add("@ChemicalParameter", permission.Chemical, DbType.Boolean);
        parameters.Add("@PermissionIdParameter", permission.PermissionId, DbType.Int32);

        return await dapper.ExecuteSqlAsync(_updatePermissionSql, parameters);
    }
}
