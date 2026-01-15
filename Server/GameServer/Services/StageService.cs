using GameServer.Models.DTOs;
using GameServer.Services.interfaces;

namespace GameServer.Services;

public class StageService : IStageService
{
    public async Task<ApiResponse<StageClearResponseDto>> StageClearAsync(long accountId, StageClearRequestDto stageClearDto)
    {
        Console.WriteLine($"[{nameof(StageService)}.{nameof(StageClearAsync)}] AccountId: {accountId}");

        var response = new ApiResponse<StageClearResponseDto>();

 

        await Task.Delay(100);

        return response;
    }
}
