using Dapper;
using GameServer.Models.DbModels;
using GameServer.Repositories.Interfaces;
using GameServer.Repositories.Queries;
using MySqlConnector;

namespace GameServer.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly string? _connectionString;
    public PlayerRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySql");
    }

    public async Task CreatePlayerAsync(PlayerEntity playerInfo)
    {
        await using var connection = new MySqlConnection(_connectionString);

        await connection.ExecuteAsync(PlayerQuery.InsertPlayer, playerInfo);
    }

    public async Task<PlayerEntity?> GetPlayerByAccountIdAsync(long accountId)
    {
        await using var connection = new MySqlConnection(_connectionString);

        var playerInfo = await connection.QueryFirstOrDefaultAsync<PlayerEntity>(PlayerQuery.LoadPlayer, new { AccountId = accountId });
        return playerInfo;

    }

    public async Task<bool> UpdatePlayerAsync(PlayerEntity playerInfo)
    {
        await using var connection = new MySqlConnection(_connectionString);

        var result = await connection.ExecuteAsync(PlayerQuery.UpdatePlayer, playerInfo);
        return result == 1;
    }

    public async Task<UpdateStageResult?> UpdateStageAsync(long accountId, int currentStage, int stageDelta, long goldReward)
    {
        await using var connection = new MySqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<UpdateStageResult>(PlayerQuery.UpdateStageAndGold, new { 
            AccountId = accountId, 
            CurrentStage = currentStage, 
            StageDelta = stageDelta
        });
    }

    public async Task<long?> UpdatePlayerLevelAsync(long accountId, int currentPlayerLevel, int playerLevelDelta, long cost)
    {
        await using var connection = new MySqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<long?>(PlayerQuery.UpdatePlayerLevelAndGold, new
        {
            AccountId = accountId,
            CurrentPlayerLevel = currentPlayerLevel,
            PlayerLevelDelta = playerLevelDelta,
            Cost = cost
        });
    }

    public async Task<long?> UpdateGoldLevelAsync(long accountId, int currentGoldLevel, int goldLevelDelta, long cost)
    {
        await using var connection = new MySqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<long?>(PlayerQuery.UpdateGoldLevelAndGold, new
        {
            AccountId = accountId,
            CurrentGoldLevel = currentGoldLevel,
            GoldLevelDelta = goldLevelDelta,
            Cost = cost
        });
    }
}
