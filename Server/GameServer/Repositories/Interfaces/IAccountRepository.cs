using GameServer.Domain.Entities;

namespace GameServer.Repositories.Interfaces;

public interface IAccountRepository
{
    Task CreateAccount(AccountEntity account);
    Task<bool> Exists(string email);
    Task<AccountEntity?> GetAccount(string email);
    Task Login(long accountId);
}
