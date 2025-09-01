namespace Laboratoire.Application.Utils;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public ApiError? Error { get; set; }

    public ApiResponse(T? data, ApiError? error)
    {
        Data = data;
        Error = error;
    }

    public static ApiResponse<T> Success(T data)
    => new ApiResponse<T>(data, null);

    public static ApiResponse<T> Failure(string message, int code, string? details = null)
    => new ApiResponse<T>(default, new ApiError(message, code, details));
}
