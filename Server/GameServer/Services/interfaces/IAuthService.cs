using GameServer.Models.DTOs;

namespace GameServer.Services.interfaces;

public interface IAuthService
{
    Task<ApiResponse> RegisterAsync(AccountDto requestDto);
    Task<ApiResponse<AccountDto>> LoginAsync(AccountDto requestDto);
    Task<ApiResponse> RenewSessionTtl(string token);
}
