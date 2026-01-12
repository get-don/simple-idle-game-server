using GameServer.Domain.Entities;
using GameServer.Repositories.Interfaces;
using GameServer.Repositories.Queries;
using MySql.Data.MySqlClient;
using static Dapper.SqlMapper;

namespace GameServer.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly string? _connectionString;

    public AccountRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConnection");
    }

    public async Task CreateAccount(AccountEntity account)
    {
        await using var connection = new MySqlConnection(_connectionString);

        account.AccountId = await connection.ExecuteScalarAsync<long>(AccountQuery.InsertAccount, account);        
    }

    public async Task<bool> Exists(string email)
    {
        await using var connection = new MySqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<bool>(AccountQuery.ExistsAccount, new { Email = email });
    }

    public async Task<AccountEntity?> GetAccount(string email)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var account = await connection.QueryFirstOrDefaultAsync<AccountEntity>(AccountQuery.LoadAccount, new { Email = email });
        return account;
    }

    public async Task Login(long accountId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(AccountQuery.UpdateLoginTime, new { @Aid = accountId});
    }
}
