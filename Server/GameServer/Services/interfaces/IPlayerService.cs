using GameServer.Models.DTOs;

namespace GameServer.Services.interfaces;

public interface IPlayerService
{
    public Task<ApiResponse<PlayerInfoDto>> GetPlayerInfoAsync(long accountId);
}
