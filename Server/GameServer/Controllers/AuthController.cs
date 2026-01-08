using GameServer.Domain.DTOs;
using GameServer.Domain.Entities;
using GameServer.Domain.Models;
using GameServer.Repositories.Interfaces;
using GameServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(AccountDto accountDto)
    {
        if (await _repository.Exists(accountDto.Email))
            return Conflict();

        var account = new AccountEntity()
        {
            Email = accountDto.Email
        };

        var passwordHasher = new PasswordHasher<AccountEntity>();
        account.Password = passwordHasher.HashPassword(new AccountEntity(), accountDto.Password);
                
        await _repository.CreateAccount(account);

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [EndpointSummary("로그인")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(AccountDto accountDto)
    {
        var account = await _repository.GetAccount(accountDto.Email);
        if(account is null)
            return Unauthorized();

        var passwordHasher = new PasswordHasher<AccountEntity>();
        var result = passwordHasher.VerifyHashedPassword(new AccountEntity(), account.Password, accountDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("Password", "Wrong Password");
            return BadRequest(ModelState);
        }

        // 기존 접속 있다면?
        var oldSessionKey = await _cache.GetSessionTokenByAccountId(account.AccountId);
        if (oldSessionKey != null)
        {
            // 레디스에 캐시된 정보들의 TTL을 모두 변경한다.
        }

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
                Token = token
            };

            if (await _cache.SaveSession(session, TimeSpan.FromMinutes(5)))
            {
                ok = true;
                accountDto.Token = token;
                break;
            }    
        }

        if(!ok)
        {
            return StatusCode(500);
        }

        return Ok(accountDto);
    }
}
