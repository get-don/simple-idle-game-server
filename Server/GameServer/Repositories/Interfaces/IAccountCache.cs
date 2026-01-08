using GameServer.Domain.Models;
namespace GameServer.Repositories.Interfaces;

public interface IAccountCache
{
    public Task<bool> SaveSession(UserSession session , TimeSpan ttl);
    public Task<string?> GetSessionTokenByAccountId(long accountId);
    public Task<UserSession?> GetSession(string sessionToken);
}
