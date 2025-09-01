
public class ApiError
{
    public string Message { get; set; } = String.Empty;
    public int Code { get; set; }
    public string? Details { get; set; } = String.Empty;

    public ApiError(string message, int code, string? details)
    {
        Message = message;
        Code = code;
        Details = details;
    }
}