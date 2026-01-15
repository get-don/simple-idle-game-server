using GameServer.Models;
using GameServer.Repositories.Interfaces;
using System.Text.Json;

namespace GameServer.Repositories;

public class PlayerStore : IPlayerStore
{
    private readonly IRedisStore _redis;

    public PlayerStore(IRedisStore redis)
    {
        _redis = redis;
    }

    private static string Key(long playerId)
        => $"player:state:{playerId}";

    public async Task<PlayerInfo?> GetAsync(long playerId)
    {
        var json = await _redis.GetStringAsync(Key(playerId));
        return json == null
            ? null
            : JsonSerializer.Deserialize<PlayerInfo>(json);
    }

    public Task SetAsync(long playerId, PlayerInfo playerInfo)
    {
        var json = JsonSerializer.Serialize(playerInfo);
        return _redis.SetStringAsync(
            Key(playerId),
            json,
            TimeSpan.FromSeconds(30)
        );
    }

    public Task InvalidateAsync(long playerId)
        => _redis.DeleteAsync(Key(playerId));
}
