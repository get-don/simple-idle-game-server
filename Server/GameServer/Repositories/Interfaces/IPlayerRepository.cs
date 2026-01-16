using GameServer.Models.DbModels;

namespace GameServer.Repositories.Interfaces;

public sealed record UpdateStageResult(long RewardGold, long TotalGold);

public interface IPlayerRepository
{
    public Task CreatePlayerAsync(PlayerEntity playerInfo);
    public Task<PlayerEntity?> GetPlayerByAccountIdAsync(long accountId);

    public Task<bool> UpdatePlayerAsync(PlayerEntity playerInfo);
    public Task<UpdateStageResult?> UpdateStageAsync(long accountId, int currentStage, int stageDelta, long goldReward);
    public Task<long?> UpdatePlayerLevelAsync(long accountId, int currentPlayerLevel, int playerLevelDelta, long cost);
    public Task<long?> UpdateGoldLevelAsync(long accountId, int currentGoldLevel, int goldLevelDelta, long cost);
}