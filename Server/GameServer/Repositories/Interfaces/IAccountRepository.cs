using GameServer.Models.DbModels;

namespace GameServer.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<long> CreateAccountAsync(AccountEntity account);
    Task<bool> ExistsAsync(string email);
    Task<AccountEntity?> GetAccountAsync(string email);
    Task LoginAsync(long accountId);
}
