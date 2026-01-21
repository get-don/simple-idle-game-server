
using GameServer.Models.DTOs;
using StackExchange.Redis;
using System.Text.Json;

namespace GameServer.Repositories.Interfaces;

public interface IIdempotencyStore
{

    Task<bool> SetStageClearResponseAsync(long accountId, int stage, ApiResponse<StageClearResponseDto> resp);
    Task<bool> UpdateStageClearResponseAsync(long accountId, int stage, ApiResponse<StageClearResponseDto> resp);

    Task<ApiResponse<StageClearResponseDto>?> GetStageClearResponseAsync(long accountId, int stage);

    Task<bool> SetPlayerLevelUpResponseAsync(long accountId, int level, ApiResponse<PlayerLevelUpResponseDto> resp);
    Task<bool> UpdatePlayerLevelUpResponseAsync(long accountId, int level, ApiResponse<PlayerLevelUpResponseDto> resp);

    Task<ApiResponse<PlayerLevelUpResponseDto>?> GetPlayerLevelUpResponseAsync(long accountId, int level);

    Task<bool> SetGoldLevelUpResponseAsync(long accountId, int level, ApiResponse<GoldLevelUpResponseDto> resp);
    Task<bool> UpdateGoldLevelUpResponseAsync(long accountId, int level, ApiResponse<GoldLevelUpResponseDto> resp);

    Task<ApiResponse<GoldLevelUpResponseDto>?> GetGoldLevelUpResponseAsync(long accountId, int level);
}
