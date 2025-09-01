using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

public sealed class AuthRepository(DataContext dapper): IAuthRepository
{
    #region SQL queries
    private readonly string _getAuthByIdCountSql =
    $"""
    SELECT
        user_id AS {nameof(Auth.UserId)},
        password_salt AS {nameof(Auth.PasswordSalt)},
        password_hash AS {nameof(Auth.PasswordHash)}
    FROM 
        users.auth
    WHERE 
        user_id = @UserIdParameter;
    """;
    private readonly string _addAuthSql =
    $"""
    INSERT INTO users.auth
    (
        user_id,
        password_salt,
        password_hash
    )
    VALUES
    (
        @UserIdParameter,
        @PasswordSaltParameter,
        @PasswordHashParameter
    );
    """;
    private readonly string _updateAuthSql =
    $"""
    UPDATE users.auth
    SET
        password_salt = @PasswordSaltParameter,
        password_hash = @PasswordHashParameter
    WHERE 
        user_id = @UserIdParameter;
    """;
    #endregion
    public async Task<bool> DoesAuthExistsAsync(Guid? userId)
    => await GetAuthByUserIdAsync(userId) is not null;

    public async Task<Auth?> GetAuthByUserIdAsync(Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserIdParameter", userId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Auth>(_getAuthByIdCountSql, parameters);
    }
    public async Task<bool> AddAuthAsync(Auth auth)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("UserIdParameter", auth.UserId, DbType.Guid);
        parameters.Add("@PasswordSaltParameter", auth.PasswordSalt, DbType.Binary);
        parameters.Add("@PasswordHashParameter", auth.PasswordHash, DbType.Binary);

        return await dapper.ExecuteSqlAsync(_addAuthSql, parameters);
    }

    public async Task<bool> UpdateAuthAsync(Auth auth)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("UserIdParameter", auth.UserId, DbType.Guid);
        parameters.Add("@PasswordSaltParameter", auth.PasswordSalt, DbType.Binary);
        parameters.Add("@PasswordHashParameter", auth.PasswordHash, DbType.Binary);

        return await dapper.ExecuteSqlAsync(_updateAuthSql, parameters);
    }
}
