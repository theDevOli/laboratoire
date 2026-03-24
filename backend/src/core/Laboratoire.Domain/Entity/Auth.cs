using Laboratoire.Domain.Exceptions;
using Laboratoire.Domain.ObjectValues;

namespace Laboratoire.Domain.Entity;

/// <summary>
/// Represents authentication credentials within the domain.
/// 
/// This entity is responsible for associating a user identifier
/// with a secure password value object. It enforces business rules
/// to ensure that authentication data is always valid.
/// </summary>
public class Auth
{
    /// <summary>
    /// Gets the unique identifier of the authenticated user.
    /// Cannot be null or empty.
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Gets the password value object associated with the user.
    /// This should contain secure representations such as hash and salt.
    /// </summary>
    public Password Password { get; private set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class.
    /// Required for ORM tools and serialization.
    /// </summary>
    public Auth() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Auth"/> class
    /// with the specified user identifier and password.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="password">The password value object.</param>
    /// <exception cref="InvalidAuthException">
    /// Thrown when the user identifier is null or empty.
    /// </exception>
    public Auth(Guid? userId, Password password)
    {
        ChangeUserId(userId);
        Password = password;
    }

    /// <summary>
    /// Updates the user identifier after validating domain rules.
    /// </summary>
    /// <param name="userId">The new user identifier.</param>
    /// <exception cref="InvalidAuthException">
    /// Thrown when the user identifier is null or empty.
    /// </exception>
    private void ChangeUserId(Guid? userId)
    {
        if (userId == Guid.Empty)
            throw new InvalidAuthException($"{nameof(UserId)} cannot be empty.");

        if (userId is null)
            throw new InvalidAuthException($"{nameof(UserId)} cannot be null.");

        UserId = userId;
    }
}
