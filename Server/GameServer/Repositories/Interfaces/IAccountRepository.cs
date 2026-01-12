using GameServer.Domain.Entities;

namespace GameServer.Repositories.Interfaces;

public interface IAccountRepository
{
    Task CreateAccountAsync(AccountEntity account);
    Task<bool> ExistsAsync(string email);
    Task<AccountEntity?> GetAccountAsync(string email);
    Task LoginAsync(long accountId);
}
