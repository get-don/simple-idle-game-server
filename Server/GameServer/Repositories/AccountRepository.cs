using Dapper;
using GameServer.Models.DbModels;
using GameServer.Repositories.Interfaces;
using GameServer.Repositories.Queries;
using MySql.Data.MySqlClient;
using static Dapper.SqlMapper;

namespace GameServer.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly string? _connectionString;

    public AccountRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySql");
    }

    public async Task<long> CreateAccountAsync(Account account)
    {
        await using var connection = new MySqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<long>(AccountQuery.InsertAccount, account);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        await using var connection = new MySqlConnection(_connectionString);

        return await connection.ExecuteScalarAsync<bool>(AccountQuery.ExistsAccount, new { Email = email });
    }

    public async Task<Account?> GetAccountAsync(string email)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var account = await connection.QueryFirstOrDefaultAsync<Account>(AccountQuery.LoadAccount, new { Email = email });
        return account;
    }

    public async Task LoginAsync(long accountId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.ExecuteAsync(AccountQuery.UpdateLoginTime, new { AccountId = accountId});
    }
}
