
using System.Text.Json;
using Laboratoire.Application.Utils;

namespace Laboratoire.Web.Middleware;

public class CatchMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CatchMiddleware> _logger;

    public CatchMiddleware(RequestDelegate next, ILogger<CatchMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var error = ApiResponse<object>.Failure(ex.Message, 500, ex.StackTrace);
            var json = JsonSerializer.Serialize(error);
            _logger.LogError("Error on the server side, message: {Message}", ex.Message);
            await context.Response.WriteAsync(json);

        }
    }
}
