using StackExchange.Redis;

namespace GameServer.Repositories.Interfaces;

public interface IRedisStore
{
    Task<string?> GetStringAsync(string key);
    Task<bool> SetStringAsync(string key, string value, TimeSpan? ttl = null, When when = When.Always);
    Task<bool> SetStringNxAsync(string key, string value, TimeSpan ttl);
    Task<bool> DeleteAsync(string key);
    Task<long> EvaluateAsync(string script, string[] keys, RedisValue[] values);
}
