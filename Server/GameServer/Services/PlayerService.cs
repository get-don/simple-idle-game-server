using GameServer.Models.DTOs;
using GameServer.Repositories;
using GameServer.Repositories.Interfaces;
using GameServer.Services.interfaces;

namespace GameServer.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IIdempotencyStore _idemStore;

    public PlayerService(IPlayerRepository playerRepo, IIdempotencyStore idemStore)
    {
        _playerRepository = playerRepo;
        _idemStore = idemStore;
    }

    public async Task<ApiResponse<PlayerInfoDto>> GetPlayerInfoAsync(long accountId)
    {
        Console.WriteLine($"[{nameof(PlayerService)}.{nameof(GetPlayerInfoAsync)}] AccountId: {accountId}");

        var response = new ApiResponse<PlayerInfoDto>();

        var playerInfo = await _playerRepository.GetPlayerByAccountIdAsync(accountId);
        if (playerInfo is null)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.NotExistsPlayer;
            return response;
        }

        response.Result = new PlayerInfoDto
        {
            Level = playerInfo.PlayerLevel,
            GoldLevel = playerInfo.GoldLevel,
            Stage = playerInfo.Stage,
            Gold = playerInfo.Gold
        };

        Console.WriteLine($"[{nameof(PlayerService)}.{nameof(GetPlayerInfoAsync)}] AccountId: {accountId}, Level: {playerInfo.PlayerLevel}, GoldLevel: {playerInfo.GoldLevel}, Stage: {playerInfo.Stage}, Gold: {playerInfo.Gold}");

        return response;
    }

    public async Task<ApiResponse<PlayerLevelUpResponseDto>> PlayerLevelUp(long accountId, PlayerLevelUpRequestDto requestDto)
    {
        var prevResponse = await _idemStore.GetPlayerLevelUpResponseAsync(accountId, requestDto.CurrentLevel);
        if(prevResponse != null)
        {
            return prevResponse;
        }

        var response = new ApiResponse<PlayerLevelUpResponseDto>
        {
            ErrorCode = ErrorCode.RequestInProgress
        };

        if (!await _idemStore.SetPlayerLevelUpResponseAsync(accountId, requestDto.CurrentLevel, response))
        {
            return response;
        }

        long cost = requestDto.CurrentLevel * 10;

        response.Result = new PlayerLevelUpResponseDto
        {
            PrevLevel = requestDto.CurrentLevel,
            NextLevel = requestDto.CurrentLevel + 1,
            Cost = cost,
            TotalGold = await _playerRepository.UpdatePlayerLevelAsync(accountId, requestDto.CurrentLevel, 1, cost) ?? -1
        };

        if(response.Result.TotalGold > 0)
        {
            response.ErrorCode = ErrorCode.Ok;
        }
        else
        {
            response.ErrorCode = ErrorCode.LevelUpFailed;
        }

        await _idemStore.UpdatePlayerLevelUpResponseAsync(accountId, requestDto.CurrentLevel, response);

        Console.WriteLine($"[{nameof(StageService)}.{nameof(PlayerLevelUp)}] AccountId: {accountId}, CurrentLevel: {requestDto.CurrentLevel}, " +
            $"NextLevel: {response.Result.NextLevel}, Cost: {response.Result.Cost}, ToTalGold: {response.Result.TotalGold}");

        return response;
    }

    public async Task<ApiResponse<GoldLevelUpResponseDto>> GoldLevelUp(long accountId, GoldLevelUpRequestDto requestDto)
    {
        var prevResponse = await _idemStore.GetGoldLevelUpResponseAsync(accountId, requestDto.CurrentLevel);
        if (prevResponse != null)
        {
            return prevResponse;
        }

        var response = new ApiResponse<GoldLevelUpResponseDto>
        {
            ErrorCode = ErrorCode.RequestInProgress
        };

        if (!await _idemStore.SetGoldLevelUpResponseAsync(accountId, requestDto.CurrentLevel, response))
        {
            return response;
        }

        long cost = requestDto.CurrentLevel * 10;

        response.Result = new GoldLevelUpResponseDto
        {
            PrevGoldLevel = requestDto.CurrentLevel,
            NextGoldLevel = requestDto.CurrentLevel + 1,
            Cost = cost,
            TotalGold = await _playerRepository.UpdateGoldLevelAsync(accountId, requestDto.CurrentLevel, 1, cost) ?? -1
        };

        if (response.Result.TotalGold > 0)
        {
            response.ErrorCode = ErrorCode.Ok;
        }
        else
        {            
            response.ErrorCode = ErrorCode.GoldLevelUpFailed;
        }

        await _idemStore.UpdateGoldLevelUpResponseAsync(accountId, requestDto.CurrentLevel, response);

        Console.WriteLine($"[{nameof(StageService)}.{nameof(GoldLevelUp)}] AccountId: {accountId}, CurrentLevel: {requestDto.CurrentLevel}, " +
            $"NextLevel: {response.Result.NextGoldLevel}, Cost: {response.Result.Cost}, ToTalGold: {response.Result.TotalGold}");

        return response;
    }

}
