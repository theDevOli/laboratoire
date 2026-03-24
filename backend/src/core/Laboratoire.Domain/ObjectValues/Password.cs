using Laboratoire.Domain.Exceptions;

namespace Laboratoire.Domain.ObjectValues;

/// <summary>
/// Represents a secure password within the domain as a value object.
///
/// This value object encapsulates the password hash and salt,
/// ensuring that raw passwords are never exposed or stored.
/// It enforces invariants to guarantee that both values are valid
/// and immutable after creation.
/// </summary>
public sealed record Password
{
    /// <summary>
    /// Gets the cryptographic salt used to generate the password hash.
    /// </summary>
    public byte[] PasswordSalt { get; }

    /// <summary>
    /// Gets the hashed representation of the password.
    /// </summary>
    public byte[] PasswordHash { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Password"/> value object.
    /// </summary>
    /// <param name="salt">The cryptographic salt.</param>
    /// <param name="hash">The hashed password.</param>
    /// <exception cref="InvalidAuthException">
    /// Thrown when the salt or hash is null or empty.
    /// </exception>
    public Password(byte[] salt, byte[] hash)
    {
        if (salt is null)
            throw new InvalidAuthException($"{nameof(PasswordSalt)}\nPassword salt cannot be null.");

        if (hash is null)
            throw new InvalidAuthException($"{nameof(PasswordHash)}\nPassword hash cannot be null.");

        if (salt.Length == 0)
            throw new InvalidAuthException($"{nameof(PasswordSalt)}\nPassword salt length cannot be zero.");

        if (hash.Length == 0)
            throw new InvalidAuthException($"{nameof(PasswordHash)}\nPassword hash length cannot be zero.");


        PasswordSalt = (byte[])salt.Clone();
        PasswordHash = (byte[])hash.Clone();
    }

    /// <summary>
    /// Deconstructs the password value object into its components.
    /// </summary>
    /// <param name="salt">The password salt.</param>
    /// <param name="hash">The password hash.</param>
    public void Deconstruct(out byte[] salt, out byte[] hash)
    {
        salt = PasswordSalt;
        hash = PasswordHash;
    }
}
