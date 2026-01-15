using GameServer.Models.DTOs;

namespace GameServer.Services.interfaces;

public interface IAuthService
{
    public Task<ApiResponse> RegisterAsync(AccountDto accountDto);
    public Task<ApiResponse<AccountDto>> LoginAsync(AccountDto accountDto);
}
