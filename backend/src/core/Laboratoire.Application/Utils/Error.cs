namespace Laboratoire.Application.Utils;

public class Error
{
    public string? Message { get; set; }
    public int StatusCode { get; set; }

    private static Dictionary<string, int> _errors = new Dictionary<string, int>(){
        {ErrorMessage.ConflictPut,409},
        {ErrorMessage.ConflictPost,409},
        {ErrorMessage.BadRequest,400},
        {ErrorMessage.BadRequestID,400},
        {ErrorMessage.BadRequestFirstIdNull,400},
        {ErrorMessage.BadRequestIdNotNull,400},
        {ErrorMessage.IDOutRange,400},
        {ErrorMessage.DbError,500},
    };
    public static Error SetError(string? message, int statusCode)
    => new Error()
    {
        Message = message,
        StatusCode = statusCode
    };
    public static Error SetError(string? message)
    => new Error()
    {
         Message = message,
        StatusCode = _errors[message!]
    };
    public static Error SetSuccess()
    => new Error()
    {
        Message = ErrorMessage.None,
        StatusCode = 0
    };
    public bool IsNotSuccess()
    => this.Message is not null && this.StatusCode != 0;
}
