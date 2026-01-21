using GameServer.Models.DTOs;
using GameServer.Repositories.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace GameServer.Repositories;

public class IdempotencyStore : IIdempotencyStore
{
    private readonly IRedisStore _redis;
    private readonly TimeSpan _ttl;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string StageClearKey(long accountId, int stage) => $"player:stage:{accountId}:{stage}";
    public string PlayerLevelUpKey(long accountId, int level) => $"player:level:{accountId}:{level}";
    public string GoldLevelUpKey(long accountId, int level) => $"player:goldlevel:{accountId}:{level}";

    public IdempotencyStore(IRedisStore redis)
    {
        _redis = redis;
        _ttl = TimeSpan.FromSeconds(10);
    }

    private async Task<bool> SetResponseAsync<T>(string key, ApiResponse<T> resp) where T : class
    {
        var json = JsonSerializer.Serialize(resp, _jsonOptions);

        return await _redis.SetStringNxAsync(
            key: key,
            value: json,
            ttl: _ttl
            );
    }

    private async Task<bool> SetResponseKeepTtlAsync<T>(string key, ApiResponse<T> resp) where T : class
    {
        var json = JsonSerializer.Serialize(resp, _jsonOptions);

        return await _redis.SetStringAsync(
            key: key,
            value: json,
            ttl: _ttl,
            when: When.Exists
            );
    }

    private async Task<ApiResponse<T>?> GetResponseAsync<T>(string key) where T : class
    {
        var value = await _redis.GetStringAsync(key);
        
        if (value is null)
            return null;

        try
        {
            return JsonSerializer.Deserialize<ApiResponse<T>>(value, _jsonOptions);
        }
        catch
        {
            // 캐시된 것이 잘못되었다면 삭제할까? ttl이 짧으니 그냥 놔둬야 할까?
            return null;
        }
    }

    public async Task<bool> SetStageClearResponseAsync(long accountId, int stage, ApiResponse<StageClearResponseDto> resp)
        => await SetResponseAsync(StageClearKey(accountId, stage), resp);

    public async Task<bool> UpdateStageClearResponseAsync(long accountId, int stage, ApiResponse<StageClearResponseDto> resp)
        => await SetResponseKeepTtlAsync(StageClearKey(accountId, stage), resp);

    public async Task<ApiResponse<StageClearResponseDto>?> GetStageClearResponseAsync(long accountId, int stage)
        => await GetResponseAsync<StageClearResponseDto>(StageClearKey(accountId, stage));

    public async Task<bool> SetPlayerLevelUpResponseAsync(long accountId, int level, ApiResponse<PlayerLevelUpResponseDto> resp)
        => await SetResponseAsync(PlayerLevelUpKey(accountId, level), resp);

    public async Task<bool> UpdatePlayerLevelUpResponseAsync(long accountId, int level, ApiResponse<PlayerLevelUpResponseDto> resp)
         => await SetResponseKeepTtlAsync(PlayerLevelUpKey(accountId, level), resp);

    public async Task<ApiResponse<PlayerLevelUpResponseDto>?> GetPlayerLevelUpResponseAsync(long accountId, int level)
        => await GetResponseAsync<PlayerLevelUpResponseDto>(PlayerLevelUpKey(accountId, level));

    public async Task<bool> SetGoldLevelUpResponseAsync(long accountId, int level, ApiResponse<GoldLevelUpResponseDto> resp)
        => await SetResponseAsync(GoldLevelUpKey(accountId, level), resp);

    public async Task<bool> UpdateGoldLevelUpResponseAsync(long accountId, int level, ApiResponse<GoldLevelUpResponseDto> resp)
          => await SetResponseKeepTtlAsync(GoldLevelUpKey(accountId, level), resp);

    public async Task<ApiResponse<GoldLevelUpResponseDto>?> GetGoldLevelUpResponseAsync(long accountId, int level)
        => await GetResponseAsync<GoldLevelUpResponseDto>(GoldLevelUpKey(accountId, level));
}
