using GameServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace GameServer.Middlewares;

/*
 * 로그인 성공 시 생성된 Token 검증 미들웨어
 */
public class SessionAuthMiddleware : IMiddleware
{
    private readonly string[] _skipPaths;
    private readonly IAccountCache _accountCache;

    public SessionAuthMiddleware(IConfiguration config, IAccountCache accountCache)
    {
        _skipPaths = config.GetSection("AuthSkipPaths").Get<string[]>() ?? [];
        _accountCache = accountCache;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value ?? "";
        if (ShouldSkip(path))
        {
            await next(context);
            return;
        }

        if (IsAnonymousEndpoint(context))
        {
            await next(context);
            return;
        }

        var token = ExtractSessionToken(context);
        if (string.IsNullOrWhiteSpace(token))
        {
            await Unauthorized(context, "Missing token");
            return;
        }

        var session = await _accountCache.GetSessionAsync(token);
        if(session is null)
        {
            await Unauthorized(context, "Invalid or expired token");
            return;
        }

        context.Items["AccountId"] = session.AccountId;
        
        Console.WriteLine($"[AuthMiddleware] Token: {token}, AccountId: {session.AccountId}");

        await next(context);
    }

    // appsettings.json에 지정된 endpoint 무시
    private bool ShouldSkip(string path) => _skipPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

    private static bool IsAnonymousEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null) return false;

        return endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;
    }

    // [AllowAnonymous] 속성이 지정된 endpoint 무시
    private static string? ExtractSessionToken(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Session-Token", out var token))
            return null;

        return token.ToString().Trim();
    }

    private static Task Unauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(body);
    }

}
