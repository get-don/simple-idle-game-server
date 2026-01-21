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

    private string Key(long accountId) => $"player:state:{accountId}";

    public async Task<PlayerInfo?> GetAsync(long accountId)
    {
        var json = await _redis.GetStringAsync(Key(accountId));
        return json == null
            ? null
            : JsonSerializer.Deserialize<PlayerInfo>(json);
    }

    public Task SetAsync(long accountId, PlayerInfo playerInfo)
    {
        var json = JsonSerializer.Serialize(playerInfo);
        return _redis.SetStringAsync(
            Key(accountId),
            json,
            TimeSpan.FromSeconds(30)
        );
    }

    public Task InvalidateAsync(long accountId)
        => _redis.DeleteAsync(Key(accountId));
}
