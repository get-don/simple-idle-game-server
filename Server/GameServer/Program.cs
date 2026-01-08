using GameServer.Middlewares;
using GameServer.Repositories;
using GameServer.Repositories.Interfaces;
using GameServer.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<OpenApiSessionTokenSecurityTransformer>();
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountCache, AccountCache>();

builder.Services.AddScoped<SessionAuthMiddleware>();

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
