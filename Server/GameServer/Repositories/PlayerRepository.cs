using Dapper;
using GameServer.Models.DbModels;
using GameServer.Repositories.Interfaces;
using GameServer.Repositories.Queries;
using MySql.Data.MySqlClient;

namespace GameServer.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly string? _connectionString;
    public PlayerRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySql");
    }

    public async Task CreatePlayerAsync(PlayerInfo playerInfo)
    {
        await using var connection = new MySqlConnection(_connectionString);

        await connection.ExecuteAsync(PlayerQuery.InsertPlayer, playerInfo);
    }

    public async Task<PlayerInfo?> GetPlayerByAccountIdAsync(long accountId)
    {
        await using var connection = new MySqlConnection(_connectionString);

        var playerInfo = await connection.QueryFirstOrDefaultAsync<PlayerInfo>(PlayerQuery.LoadPlayer, new { AccountId = accountId });
        return playerInfo;

    }

    public async Task UpdatePlayerAsync(PlayerInfo playerInfo)
    {
        await using var connection = new MySqlConnection(_connectionString);

        await connection.QueryAsync(PlayerQuery.UpdatePlayer, playerInfo);
    }
}
