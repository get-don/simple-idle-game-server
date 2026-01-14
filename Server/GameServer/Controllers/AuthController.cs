using GameServer.Repositories.Interfaces;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DbModels;
using GameServer.Models.DTOs;

namespace GameServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountRepository _repository;
    private readonly IAccountCache _cache;

    public AuthController(IAccountRepository repo, IAccountCache cache)
    {
        _repository = repo;
        _cache = cache;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    [EndpointSummary("계정 등록")]
   // [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> Register(AccountDto accountDto)
    {
        Console.WriteLine($"[Register] Email: {accountDto.Email}, Password: {accountDto.Password}");

        var response = new ApiResponse();

        if (await _repository.ExistsAsync(accountDto.Email))
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.EmailAlreadyExists;
            return Ok(response);
        }

        var account = new Account()
        {
            Email = accountDto.Email
        };

        var passwordHasher = new PasswordHasher<Account>();
        account.Password = passwordHasher.HashPassword(new Account(), accountDto.Password);
                
        await _repository.CreateAccountAsync(account);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [EndpointSummary("로그인")]
   // [ProducesResponseType(typeof(ApiResponse<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccountDto>>> Login(AccountDto accountDto)
    {
        Console.WriteLine($"[Login] Email: {accountDto.Email}, Password: {accountDto.Password}");

        var response = new ApiResponse<AccountDto>();

        var account = await _repository.GetAccountAsync(accountDto.Email);
        if(account is null)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.AccountNotExist;
            return Ok(response);
        }

        var passwordHasher = new PasswordHasher<Account>();
        var result = passwordHasher.VerifyHashedPassword(new Account(), account.Password, accountDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.WrongPassword;
            return Ok(response);
        }

        // 기존 접속이 있다면?
        var isAlreadyConnected = await _cache.GetSessionTokenByAccountIdAsync(account.AccountId);

        // 방어 코드
        bool ok = false;
        for(var i = 0; i < 5; i++)
        {
            // 세션 토큰 생성
            var token = TokenGenerator.CreateSessionToken();

            var session = new UserSession()
            {
                AccountId = account.AccountId,
                Email = account.Email,
                Token = token,
                LoginTime = DateTime.UtcNow
            };

            if (await _cache.TryCreateSessionAsync(session, TimeSpan.FromMinutes(5)))
            {
                ok = true;
                accountDto.Password = "";
                accountDto.Token = token;
                break;
            }    
        }

        if(!ok)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.InternalServerError;
            return Ok(response);
        }

        if (isAlreadyConnected != null)
        {
            // 레디스에 캐시된 정보들의 TTL을 모두 변경한다.
        }

        await _repository.LoginAsync(account.AccountId);

        Console.WriteLine($"[Login] Success Email: {accountDto.Email}, Token: {accountDto.Token}");

        response.Result = accountDto;
        return Ok(response);
    }
}
