using GameServer.Models.DbModels;

namespace GameServer.Repositories.Interfaces;

public interface IPlayerRepository
{
    public Task CreatePlayerAsync(PlayerEntity playerInfo);
    public Task<PlayerEntity?> GetPlayerByAccountIdAsync(long accountId);

    public Task UpdatePlayerAsync(PlayerEntity playerInfo);
}