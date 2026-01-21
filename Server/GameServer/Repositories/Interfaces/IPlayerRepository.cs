using GameServer.Models.DbModels;

namespace GameServer.Repositories.Interfaces;

public sealed record UpdateStageResult(long RewardGold, long TotalGold);

public interface IPlayerRepository
{
    Task CreatePlayerAsync(PlayerEntity playerInfo);
    Task<PlayerEntity?> GetPlayerByAccountIdAsync(long accountId);

    Task<bool> UpdatePlayerAsync(PlayerEntity playerInfo);
    Task<UpdateStageResult?> UpdateStageAsync(long accountId, int currentStage, int stageDelta, long goldReward);
    Task<long?> UpdatePlayerLevelAsync(long accountId, int currentPlayerLevel, int playerLevelDelta, long cost);
    Task<long?> UpdateGoldLevelAsync(long accountId, int currentGoldLevel, int goldLevelDelta, long cost);
}