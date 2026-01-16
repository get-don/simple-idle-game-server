using GameServer.Models.DTOs;
using GameServer.Repositories.Interfaces;
using GameServer.Services.interfaces;

namespace GameServer.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepo)
    {
        _playerRepository = playerRepo;
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
        long cost = requestDto.CurrentLevel * 10;

        var response = new ApiResponse<PlayerLevelUpResponseDto>
        {                   
            Result = new PlayerLevelUpResponseDto
            {
                NextLevel = requestDto.CurrentLevel,
                Cost = cost,
                TotalGold = await _playerRepository.UpdatePlayerLevelAsync(accountId, requestDto.CurrentLevel, 1, cost) ?? -1
            }
        };

        if(response.Result.TotalGold < 0)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.LevelUpFailed;
        }

        Console.WriteLine($"[{nameof(StageService)}.{nameof(PlayerLevelUp)}] AccountId: {accountId}, CurrentLevel: {requestDto.CurrentLevel}, " +
            $"NextLevel: {response.Result.NextLevel}, Cost: {response.Result.Cost}, ToTalGold: {response.Result.TotalGold}");

        return response;
    }

    public async Task<ApiResponse<GoldLevelUpResponseDto>> GoldLevelUp(long accountId, GoldLevelUpRequestDto requestDto)
    {
        long cost = requestDto.CurrentLevel * 10;

        var response = new ApiResponse<GoldLevelUpResponseDto>
        {
            Result = new GoldLevelUpResponseDto
            {
                NextGoldLevel = requestDto.CurrentLevel,
                Cost = cost,
                TotalGold = await _playerRepository.UpdateGoldLevelAsync(accountId, requestDto.CurrentLevel, 1, cost) ?? -1
            }
        };

        if (response.Result.TotalGold < 0)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.GoldLevelUpFailed;
        }

        Console.WriteLine($"[{nameof(StageService)}.{nameof(GoldLevelUp)}] AccountId: {accountId}, CurrentLevel: {requestDto.CurrentLevel}, " +
            $"NextLevel: {response.Result.NextGoldLevel}, Cost: {response.Result.Cost}, ToTalGold: {response.Result.TotalGold}");

        return response;
    }

}
