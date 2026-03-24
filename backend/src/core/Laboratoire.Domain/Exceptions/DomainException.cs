namespace Laboratoire.Domain.Exceptions;

/// <summary>
/// Represents the base exception type for all domain-related errors.
///
/// This exception is intended to be used within the domain layer to signal
/// business rule violations or invalid operations according to domain logic.
/// 
/// All custom domain exceptions should inherit from this class to provide
/// consistency and allow centralized handling of domain errors.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    public DomainException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class
    /// with a specified error message describing the exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DomainException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class
    /// with a specified error message and a reference to the inner exception
    /// that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that caused the current exception.
    /// </param>
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
