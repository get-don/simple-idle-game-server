using GameServer.Models.DTOs;

namespace GameServer.Services.interfaces;

public interface IStageService
{
    public Task<ApiResponse<StageClearResponseDto>> StageClearAsync(long accountId, StageClearRequestDto requestDto);
}
