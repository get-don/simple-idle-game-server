using GameServer.Models.DbModels;

namespace GameServer.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<long> CreateAccountAsync(Account account);
    Task<bool> ExistsAsync(string email);
    Task<Account?> GetAccountAsync(string email);
    Task LoginAsync(long accountId);
}
