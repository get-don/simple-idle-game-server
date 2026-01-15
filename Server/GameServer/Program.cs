using GameServer.Middlewares;
using GameServer.Models.DbModels;
using GameServer.Repositories;
using GameServer.Repositories.Interfaces;
using GameServer.Services;
using GameServer.Services.interfaces;
using GameServer.Utils;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var redis = cfg.GetConnectionString("Redis");

    if (string.IsNullOrWhiteSpace(redis))
        throw new InvalidOperationException("ConnectionStrings:Redis is empty. Check Secret/ENV.");

    return ConnectionMultiplexer.Connect(redis);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiSessionTokenSecurityTransformer>();
});

builder.Services.AddSingleton<AppState>();
builder.Services.AddHostedService<AppInitializer>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddScoped<IRedisStore, RedisStore>();
builder.Services.AddScoped<IAccountStore, AccountStore>();

builder.Services.AddScoped<SessionAuthMiddleware>();

builder.Services.AddSingleton<IPasswordHasher<AccountEntity>, PasswordHasher<AccountEntity>>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IStageService, StageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "api");
    });
}

app.UseMiddleware<SessionAuthMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
