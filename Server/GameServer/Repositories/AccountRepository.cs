using GameServer.Domain.Entities;
using GameServer.Repositories.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace GameServer.Repositories;

public class AccountRepository : IAccountRepository
{
    // 테스트 코드
    private static Dictionary<string, AccountEntity> _testAccount = [];
    private static long _testAccountIdGenerator = 1000;

    public async Task CreateAccount(AccountEntity account)
    {
        await Task.Delay(100);

        _testAccount.Add(account.Email, account);
        account.AccountId = _testAccountIdGenerator++;
    }

    public async Task<bool> Exists(string email)
    {
        await Task.Delay(100);

        return _testAccount.ContainsKey(email);
    }

    public async Task<AccountEntity?> GetAccount(string email)
    {
        await Task.Delay(100);

        if(_testAccount.TryGetValue(email, out var acount))
            return acount;

        return null;
    }

    public async Task Login(long accountId)
    {
        await Task.Delay(100);
    }
}
