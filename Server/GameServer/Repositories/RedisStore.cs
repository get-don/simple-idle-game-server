using GameServer.Repositories.Interfaces;
using StackExchange.Redis;

namespace GameServer.Repositories;

public class RedisStore : IRedisStore
{
    private readonly IDatabase _redis;

    public RedisStore(IConnectionMultiplexer connection)
    {
        _redis = connection.GetDatabase();
    }

    public async Task<string?> GetStringAsync(string key)
    {
        var value = await _redis.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> SetStringAsync(string key, string value, TimeSpan? ttl = null, When when = When.Always) 
        => await _redis.StringSetAsync(key, value, ttl, when);

    public async Task<bool> SetStringNxAsync(string key, string value, TimeSpan ttl) 
        => await _redis.StringSetAsync(key, value, ttl, When.NotExists);

    public async Task<bool> DeleteAsync(string key)
    => await _redis.KeyDeleteAsync(key);

    public async Task<long> EvaluateAsync(string script, string[] keys, RedisValue[] values)
    {
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        var result = await _redis.ScriptEvaluateAsync(script, redisKeys, values);
        return (long)result;
    }
}
