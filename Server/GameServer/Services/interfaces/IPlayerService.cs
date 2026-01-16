using GameServer.Models.DTOs;

namespace GameServer.Services.interfaces;

public interface IPlayerService
{
    public Task<ApiResponse<PlayerInfoDto>> GetPlayerInfoAsync(long accountId);
    public Task<ApiResponse<PlayerLevelUpResponseDto>> PlayerLevelUp(long accountId, PlayerLevelUpRequestDto requestDto);
    public Task<ApiResponse<GoldLevelUpResponseDto>> GoldLevelUp(long accountId, GoldLevelUpRequestDto requestDto);
}
