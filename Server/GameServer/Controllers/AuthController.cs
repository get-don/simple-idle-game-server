using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTOs;
using GameServer.Services.interfaces;

namespace GameServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    [EndpointSummary("계정 등록")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> Register(AccountDto requestDto)
    {
        var response = await _authService.RegisterAsync(requestDto);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [EndpointSummary("로그인")]
    [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccountDto>>> Login(AccountDto requestDto)
    {
        var response = await _authService.LoginAsync(requestDto);
        return Ok(response);
    }
}
