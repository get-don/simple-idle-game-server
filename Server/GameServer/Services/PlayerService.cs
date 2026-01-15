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
        var response = new ApiResponse<PlayerInfoDto>();

        Console.WriteLine($"[{nameof(PlayerService)}.{nameof(GetPlayerInfoAsync)}] AccountId: {accountId}");

        var playerInfo = await _playerRepository.GetPlayerByAccountIdAsync(accountId);
        if (playerInfo is null)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.NotExistsPlayer;
            return response;
        }

        response.Result = new PlayerInfoDto
        {
            Level = playerInfo.Level,
            GoldLevel = playerInfo.GoldLevel,
            Stage = playerInfo.Stage,
            Gold = playerInfo.Gold
        };

        Console.WriteLine($"[{nameof(PlayerService)}.{nameof(GetPlayerInfoAsync)}] AccountId: {accountId}, Level: {playerInfo.Level}, GoldLevel: {playerInfo.GoldLevel}, Stage: {playerInfo.Stage}, Gold: {playerInfo.Gold}");

        return response;
    }
}
