using GameServer.Repositories.Interfaces;

namespace GameServer.Repositories;

public class RedisIdempotencyStore : IIdempotencyStore
{
    private readonly IRedisStore _redis;

    public RedisIdempotencyStore(IRedisStore redis)
    {
        _redis = redis;
    }

    private static string Key(long userId, string requestId)
        => $"idem:{userId}:{requestId}";

    public Task<string?> GetAsync(long userId, string requestId)
        => _redis.GetStringAsync(Key(userId, requestId));

    // processing 상태 선점 (NX)
    public Task<bool> TryBeginAsync(long userId, string requestId)
        => _redis.SetStringNxAsync(
            Key(userId, requestId),
            "processing",
            TimeSpan.FromSeconds(30)
        );

    // 처리 완료 → 응답 저장 (TTL 유지)
    public Task CompleteAsync(long userId, string requestId, string responseJson)
        => _redis.SetStringAsync(
            Key(userId, requestId),
            responseJson,
            TimeSpan.FromMinutes(5)
        );

    // 실패 시 재시도 허용
    public Task ClearAsync(long userId, string requestId)
        => _redis.DeleteAsync(Key(userId, requestId));
}
