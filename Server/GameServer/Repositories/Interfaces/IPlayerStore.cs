using GameServer.Models;

namespace GameServer.Repositories.Interfaces;

public interface IPlayerStore
{
    Task<PlayerInfo?> GetAsync(long playerId);
    Task SetAsync(long playerId, PlayerInfo playerInfo);
    Task InvalidateAsync(long playerId);
}
