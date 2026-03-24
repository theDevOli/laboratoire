
namespace Laboratoire.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when authentication-related
/// domain rules are violated.
///
/// This exception is used to indicate invalid authentication states,
/// such as missing or incorrect user identifiers, invalid credentials,
/// or any inconsistency within the authentication entity.
/// </summary>
public class InvalidAuthException : DomainException
{

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAuthException"/> class.
    /// </summary>
    public InvalidAuthException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAuthException"/> class
    /// with a specified error message describing the validation failure.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidAuthException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidAuthException"/> class
    /// with a specified error message and a reference to the inner exception
    /// that caused this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that caused the current exception.
    /// </param>
    public InvalidAuthException(string message, Exception innerException) : base(message, innerException) { }
}
