using System.Diagnostics;
using System.Text.Json;

namespace GameServer.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}, Path={Path}, Method={Method}",
                traceId, context.Request.Path, context.Request.Method);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            var (statusCode, title) = MakeInfo(ex);
            context.Response.StatusCode = statusCode;

            object body = _env.IsDevelopment()
                ? new
                {
                    title,
                    status = statusCode,
                    traceId,
                    detail = ex.Message,
                    exception = ex.GetType().FullName,
                    stackTrace = ex.StackTrace
                }
                : new
                {
                    title,
                    status = statusCode,
                    traceId
                };

            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }

    private static (int statusCode, string title) MakeInfo(Exception ex)
    {
        return ex switch
        {
            ArgumentException or FormatException =>
                (StatusCodes.Status400BadRequest, "Bad Request"),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Not Found"),

            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized"),

            _ =>
                (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
    }
}
