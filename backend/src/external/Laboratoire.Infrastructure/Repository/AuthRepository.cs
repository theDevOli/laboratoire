using System.Data;
using Dapper;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.ObjectValues;
using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Infrastructure.DbContext;

namespace Laboratoire.Infrastructure.Repository;

/// <summary>
/// Provides data access operations for <see cref="Auth"/> entities using Dapper.
///
/// This repository is responsible for persisting and retrieving authentication
/// data from the database. It encapsulates all SQL operations related to the
/// <c>users.auth</c> table and maps database records to domain entities.
/// </summary>
public sealed class AuthRepository(DataContext dapper) : IAuthRepository
{
    #region SQL queries

    /// <summary>
    /// SQL query to retrieve authentication data by user identifier.
    /// </summary>
    private readonly string _getAuthByIdCountSql =
    $"""
    SELECT
        user_id AS {nameof(Auth.UserId)},
        password_salt AS {nameof(Password.PasswordSalt)},
        password_hash AS {nameof(Password.PasswordHash)}
    FROM 
        users.auth
    WHERE 
        user_id = @UserIdParameter;
    """;

    /// <summary>
    /// SQL query to insert a new authentication record.
    /// </summary>
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

    /// <summary>
    /// SQL query to update an existing authentication record.
    /// </summary>
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

    /// <summary>
    /// Determines whether authentication data exists for a given user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// <c>true</c> if authentication data exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> DoesAuthExistsAsync(Guid? userId)
    => await GetAuthByUserIdAsync(userId) is not null;

    /// <summary>
    /// Retrieves authentication data for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// An <see cref="Auth"/> instance if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<Auth?> GetAuthByUserIdAsync(Guid? userId)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserIdParameter", userId, DbType.Guid);

        return await dapper.LoadDataSingleAsync<Auth>(_getAuthByIdCountSql, parameters);
    }

    /// <summary>
    /// Inserts a new authentication record into the database.
    /// </summary>
    /// <param name="auth">The authentication entity to persist.</param>
    /// <returns>
    /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> AddAuthAsync(Auth auth)
    {
        DynamicParameters parameters = new DynamicParameters();

        var (salt, hash) = auth.Password;

        parameters.Add("UserIdParameter", auth.UserId, DbType.Guid);
        parameters.Add("@PasswordSaltParameter", salt, DbType.Binary);
        parameters.Add("@PasswordHashParameter", hash, DbType.Binary);

        return await dapper.ExecuteSqlAsync(_addAuthSql, parameters);
    }

    /// <summary>
    /// Updates an existing authentication record.
    /// </summary>
    /// <param name="auth">The authentication entity containing updated data.</param>
    /// <returns>
    /// <c>true</c> if the update was successful; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> UpdateAuthAsync(Auth auth)
    {
        DynamicParameters parameters = new DynamicParameters();

        var (salt, hash) = auth.Password;

        parameters.Add("UserIdParameter", auth.UserId, DbType.Guid);
        parameters.Add("@PasswordSaltParameter", salt, DbType.Binary);
        parameters.Add("@PasswordHashParameter", hash, DbType.Binary);

        return await dapper.ExecuteSqlAsync(_updateAuthSql, parameters);
    }
}
