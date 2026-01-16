using GameServer.Models.DTOs;
using GameServer.Repositories.Interfaces;
using GameServer.Services.interfaces;

namespace GameServer.Services;

public class StageService : IStageService
{
    private readonly IPlayerRepository _playerRepository;

    public StageService(IPlayerRepository playerRepo)
    {
        _playerRepository = playerRepo;
    }

    public async Task<ApiResponse<StageClearResponseDto>> StageClearAsync(long accountId, StageClearRequestDto requestDto)
    {
        var response = new ApiResponse<StageClearResponseDto>
        {           
            Result = new StageClearResponseDto
            {
                ClearStage = requestDto.ClearStage,
                NextStage = requestDto.ClearStage + 1,
            }            
        };

        // Reward는 간단해서 그냥 쿼리에서 계산함.
        var result = await _playerRepository.UpdateStageAsync(accountId, requestDto.ClearStage, 1, 10);
        if(result is null)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.StageMismatch;
        }
        else
        {
            response.Result.RewardGold = result.RewardGold;
            response.Result.TotalGold = result.TotalGold;
        }

        Console.WriteLine($"[{nameof(StageService)}.{nameof(StageClearAsync)}] AccountId: {accountId}, Stage: {requestDto.ClearStage}, " +
            $"Reward: {response.Result.RewardGold}, TotalGold: {response.Result.TotalGold}");

        return response;
    }
}
