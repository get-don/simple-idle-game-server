using GameServer.Models.DbModels;

namespace GameServer.Repositories.Interfaces;

public interface IPlayerRepository
{
    public Task CreatePlayerAsync(PlayerInfo playerInfo);
    public Task<PlayerInfo?> GetPlayerByAccountIdAsync(long accountId);

    public Task UpdatePlayerAsync(PlayerInfo playerInfo);
}