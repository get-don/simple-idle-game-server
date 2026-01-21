using GameServer.Models;

namespace GameServer.Repositories.Interfaces;

public interface IAccountStore
{
    Task<bool> TryCreateSessionAsync(UserSession session);
    Task<string?> GetSessionTokenByAccountIdAsync(long accountId);
    Task<UserSession?> GetSessionAsync(string sessionToken);
    Task RenewSessionTtl(UserSession session);
}
