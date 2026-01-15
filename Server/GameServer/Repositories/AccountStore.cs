using GameServer.Models;
using GameServer.Repositories.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace GameServer.Repositories;

public class AccountStore : IAccountStore
{
    private readonly IRedisStore _redis;
    private static readonly JsonSerializerOptions JsonOpt = new(JsonSerializerDefaults.Web);

    private static string TokenKey(string token) => $"session:token:{token}";
    private static string AccountKey(long accountId) => $"session:account:{accountId}";


    public AccountStore(IRedisStore redis)
    {
        _redis = redis;
    }

    public async Task<bool> TryCreateSessionAsync(UserSession session, TimeSpan ttl)
    {
        string tokenKey = TokenKey(session.Token);
        string accountKey = AccountKey(session.AccountId);

        string sessionJson = JsonSerializer.Serialize(session, JsonOpt);
        long ttlSeconds = (long)ttl.TotalSeconds;                    

        const string script = """
        if redis.call('EXISTS', KEYS[1]) == 1 then
          return 0
        end
        redis.call('SET', KEYS[1], ARGV[1], 'EX', ARGV[3])
        redis.call('SET', KEYS[2], ARGV[2], 'EX', ARGV[3])
        return 1
        """;

        var result = await _redis.EvaluateAsync(script,
            keys: [tokenKey, accountKey],
            values: [session.AccountId, sessionJson, ttlSeconds]
        );

        return result == 1;
    }

    public async Task<UserSession?> GetSessionAsync(string sessionToken)
    {
        var accountIdValue = await _redis.GetStringAsync(TokenKey(sessionToken));

        if (accountIdValue is null)
            return null;

        if (!long.TryParse(accountIdValue, out var accountId))
            return null;

        var sessionJson = await _redis.GetStringAsync(AccountKey(accountId));

        if (sessionJson is null)
            return null;

        return JsonSerializer.Deserialize<UserSession>(sessionJson, JsonOpt);
    }

    public async Task<string?> GetSessionTokenByAccountIdAsync(long accountId)
    {
        var sessionJson = await _redis.GetStringAsync(AccountKey(accountId));

        if (sessionJson is null)
            return null;

        var session = JsonSerializer.Deserialize<UserSession>(sessionJson, JsonOpt);

        return session?.Token;
    }


}