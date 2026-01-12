using GameServer.Domain.Models;
namespace GameServer.Repositories.Interfaces;

public interface IAccountCache
{
    public Task<bool> TryCreateSessionAsync(UserSession session , TimeSpan ttl);
    public Task<string?> GetSessionTokenByAccountIdAsync(long accountId);
    public Task<UserSession?> GetSessionAsync(string sessionToken);
}
