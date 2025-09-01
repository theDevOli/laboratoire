namespace Laboratoire.Application.Utils;

public static class ErrorMessage
{
    public readonly static string? None = null;
    public readonly static string ConflictPost = "The record to be created already exists.";
    public readonly static string ConflictPut = "The record to be updated has conflicting information.";
    public readonly static string NotFound = "The record was not found on the database.";
    public readonly static string BadRequest = "There is a problem with the sent entity.";
    public readonly static string BadRequestID = "The id from the route do not match within the id from the body request.";
    public readonly static string BadRequestFirstIdNull = "An error occurred by processing the first Collection's ID. It cannot be null.";
    public readonly static string BadRequestIdNotNull = "ID cannot be null.";
    public readonly static string IDOutRange = "The ID is out of entity's collection range.";
    public readonly static string DbError = "An error occurred during the database transaction.";
    public readonly static string Unauthorized = "Access is denied due to invalid or missing credentials. Please ensure you are authenticated and have the necessary permissions to access this resource.";
    public readonly static string Forbidden = "Your account is inactive. Please contact support for assistance.";
}
