namespace Laboratoire.Web.Middleware;

public class LogUserEnricherMiddleware
{
    private readonly RequestDelegate _next;

    public LogUserEnricherMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var userName = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Claims.FirstOrDefault()?.Value
            : "Anonymous";

        using (Serilog.Context.LogContext.PushProperty("AuthenticatedUser", userName))
        {
            await _next(context);
        }
    }
}
