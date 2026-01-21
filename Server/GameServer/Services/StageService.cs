using GameServer.Models.DTOs;
using GameServer.Repositories;
using GameServer.Repositories.Interfaces;
using GameServer.Services.interfaces;

namespace GameServer.Services;

public class StageService : IStageService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IIdempotencyStore _idemStore;

    public StageService(IPlayerRepository playerRepo, IIdempotencyStore idemStore)
    {
        _playerRepository = playerRepo;
        _idemStore = idemStore;
    }

    public async Task<ApiResponse<StageClearResponseDto>> StageClearAsync(long accountId, StageClearRequestDto requestDto)
    {
        var prevResponse = await _idemStore.GetStageClearResponseAsync(accountId, requestDto.ClearStage);
        if (prevResponse != null)
        {
            return prevResponse;
        }

        var response = new ApiResponse<StageClearResponseDto>
        {
            ErrorCode = ErrorCode.RequestInProgress
        };

        if (!await _idemStore.SetStageClearResponseAsync(accountId, requestDto.ClearStage, response))
        {
            return response;
        }


        response.Result = new StageClearResponseDto
        {
            ClearStage = requestDto.ClearStage,
            NextStage = requestDto.ClearStage + 1,
        };

        // Reward는 간단해서 그냥 쿼리에서 계산함.
        var result = await _playerRepository.UpdateStageAsync(accountId, requestDto.ClearStage, 1, 10);
        if(result is null)
        {            
            response.ErrorCode = ErrorCode.StageMismatch;
        }
        else
        {
            response.ErrorCode = ErrorCode.Ok;
            response.Result.RewardGold = result.RewardGold;
            response.Result.TotalGold = result.TotalGold;
        }

        await _idemStore.UpdateStageClearResponseAsync(accountId, requestDto.ClearStage, response);

        Console.WriteLine($"[{nameof(StageService)}.{nameof(StageClearAsync)}] AccountId: {accountId}, Stage: {requestDto.ClearStage}, " +
            $"Reward: {response.Result.RewardGold}, TotalGold: {response.Result.TotalGold}");

        return response;
    }
}
