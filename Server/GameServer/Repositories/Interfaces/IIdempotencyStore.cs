namespace GameServer.Repositories.Interfaces;

public interface IIdempotencyStore
{
    Task<string?> GetAsync(long userId, string requestId);
    Task<bool> TryBeginAsync(long userId, string requestId);
    Task CompleteAsync(long userId, string requestId, string responseJson);
    Task ClearAsync(long userId, string requestId);
}
