using System.Data;
using Dapper;

using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Infrastructure.Repository;

public sealed class UserRepository(DataContext dapper) : IUserRepository
{
    #region SQL queries
    private readonly string _getAllUsersSql =
    $"""
    SELECT
        u.user_id AS {nameof(User.UserId)},
        u.username AS {nameof(User.Username)},
        u.is_active AS {nameof(User.IsActive)},

        r.role_id AS {nameof(Role.RoleId)},
        r.role_name AS {nameof(Role.RoleName)},

        COALESCE(cc.client_name, cp.partner_name, e.name) AS Name
        FROM 
            users.user AS u
        INNER JOIN 
            users.role AS r 
            ON r.role_id = u.role_Id
        LEFT JOIN 
            customers.client AS cc 
            ON u.user_id = cc.user_id
        LEFT JOIN 
            customers.partner AS cp
            ON u.user_id = cp.user_id
        LEFT JOIN 
            employee.employee AS e
            ON u.user_id = e.user_id
        ORDER BY 
            r.role_id,u.username;
    """;
    private readonly string _getAuthenticationByUserIdSql =
    $"""
    SELECT
        u.user_id AS {nameof(User.UserId)},
        u.username AS {nameof(User.Username)},
        u.is_active AS {nameof(User.IsActive)},

        p.protocol AS {nameof(Permission.Protocol)},
        p.client AS {nameof(Permission.Client)},
        p.property AS {nameof(Permission.Property)},
        p.cash_flow AS {nameof(Permission.CashFlow)},
        p.partner AS {nameof(Permission.Partner)},
        p.users AS {nameof(Permission.Users)},
        p.chemical AS {nameof(Permission.Chemical)},

        c.client_id AS {nameof(Client.ClientId)},

        cp.partner_id AS {nameof(Partner.PartnerId)},

        COALESCE(cc.client_name, cp.partner_name, e.name) AS Name
    FROM 
        users.user AS u
    INNER JOIN 
        users.permission AS p
        ON u.role_Id = p.role_id
    LEFT JOIN 
        customers.client as c 
        ON u.username = c.client_tax_id
    LEFT JOIN 
        customers.client AS cc 
        ON u.user_id = cc.user_id
    LEFT JOIN 
        customers.partner AS cp
        ON u.user_id = cp.user_id
    LEFT JOIN 
        employee.employee AS e
        ON u.user_id = e.user_id
    WHERE 
        u.user_id = @UserIdParameter;
    """;
    private readonly string _getUserByIdSql =
    $"""
    SELECT
        user_id AS {nameof(User.UserId)},
        role_id AS {nameof(User.RoleId)},
        username AS {nameof(User.Username)},
        is_active AS {nameof(User.IsActive)}
    FROM 
        users.user
    WHERE 
        user_id = @UserIdParameter;
    """;
    private readonly string _getUserByUserNameSql =
    $"""
    SELECT
        user_id AS {nameof(User.UserId)},
        role_id AS {nameof(User.RoleId)},
        username AS {nameof(User.Username)},
        is_active AS {nameof(User.IsActive)}
    FROM 
        users.user
    WHERE 
        username = @UsernameParameter;
    """;
    private readonly string _addUserSql =
    $"""
    INSERT INTO users.user(
        role_id,
        username,
        is_active
    )
    VALUES(
        @RoleIdParameter,
        @UsernameParameter,
        @IsActiveParameter
    )
    RETURNING 
        user_id;
    """;
    private readonly string _updateUserSql =
    $"""
    UPDATE users.user
    SET
        user_id = @RoleIdParameter,
        username = @UsernameParameter,
        is_active = @IsActiveParameter
    WHERE 
        username = @UsernameParameter;
    """;

    private readonly string _renameUserSql =
    $"""
    UPDATE users.user
    SET
        username = @UsernameParameter,
    WHERE 
        user_id = @UserIdParameter;
    """;
    private readonly string _updateUserStatusSql =
    $"""
    UPDATE users.user
    SET
        is_active = @IsActiveParameter
    WHERE 
        UserId = @UserIdParameter;
    """;
    private readonly string _countUsernameSql =
    $"""
    SELECT
        COUNT(username)
    FROM 
        users.user
    WHERE 
        username LIKE @UsernameParameter;
    """;
    #endregion
    public async Task<Guid?> AddUserAsync(User user)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", user.RoleId, DbType.Int32);
        parameters.Add("@UsernameParameter", user.Username, DbType.String);
        parameters.Add("@IsActiveParameter", user.IsActive, DbType.Boolean);

        return await dapper.LoadDataSingleAsync<Guid>(_addUserSql, parameters);
    }
    public async Task<bool> DoesUserExistByIdAsync(User user)
    => await GetUserByIdAsync(user.UserId) is not null;
    public async Task<bool> DoesUserExistByUsernameAsync(User user)
    => await GetUserByUsernameAsync(user.Username) is not null;
    public async Task<IEnumerable<DisplayUser>> GetAllUsersAsync()
    => await dapper.LoadDataAsync<DisplayUser>(_getAllUsersSql);
    public async Task<Authentication?> GetAuthenticationByIdAsync(Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserIdParameter", userId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Authentication>(_getAuthenticationByUserIdSql, parameters);
    }
    public async Task<User?> GetUserByIdAsync(Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserIdParameter", userId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<User>(_getUserByIdSql, parameters);
    }
    public async Task<User?> GetUserByUsernameAsync(string? username)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UsernameParameter", username, DbType.String);

        return await dapper.LoadDataSingleAsync<User>(_getUserByUserNameSql, parameters);
    }
    public async Task<bool> UpdateUserAsync(User user)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RoleIdParameter", user.RoleId, DbType.Int32);
        parameters.Add("@UsernameParameter", user.Username, DbType.String);
        parameters.Add("@IsActiveParameter", user.IsActive, DbType.Boolean);

        return await dapper.ExecuteSqlAsync(_updateUserSql, parameters);
    }
    public async Task<bool> UpdateUserStatusAsync(User user)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@IsActiveParameter", user.IsActive, DbType.Boolean);
        parameters.Add("@UserIdParameter", user.UserId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_updateUserStatusSql, parameters);
    }
    public async Task<bool> UserRenameAsync(User user)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UsernameParameter", user.Username, DbType.String);
        parameters.Add("@UserIdParameter", user.UserId, DbType.Guid);

        return await dapper.ExecuteSqlAsync(_renameUserSql, parameters);
    }
    public async Task<string> SetUserNameAsync(string? username)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UsernameParameter", $"%{username}%", DbType.String);

        var count = await dapper.LoadDataSingleAsync<int>(_countUsernameSql, parameters);

        return $"{username}0{count + 1}";
    }
}
