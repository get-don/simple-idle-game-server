using GameServer.Models;
using GameServer.Models.DbModels;
using GameServer.Models.DTOs;
using GameServer.Repositories.Interfaces;
using GameServer.Services.interfaces;
using GameServer.Utils;
using Microsoft.AspNetCore.Identity;

namespace GameServer.Services;

public class AuthService : IAuthService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountStore _accountStore;

    private readonly IPasswordHasher<AccountEntity> _passwordHasher;

    public AuthService(IPlayerRepository playerRepository, IAccountRepository accountRepository, IAccountStore accountStore, IPasswordHasher<AccountEntity> passwordHasher)
    {
        _playerRepository = playerRepository;
        _accountRepository = accountRepository;
        _accountStore = accountStore;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse> RegisterAsync(AccountDto accountDto)
    {
        // Note: 디버깅용 로그이므로 Logger 사용 안함
        Console.WriteLine($"[{nameof(AuthService)}.{nameof(RegisterAsync)}] Email: {accountDto.Email}, Password: {accountDto.Password}");

        var response = new ApiResponse();

        if (await _accountRepository.ExistsAsync(accountDto.Email))
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.EmailAlreadyExists;
            return response;
        }

        var account = new AccountEntity
        {
            Email = accountDto.Email,
        };

        account.Password = _passwordHasher.HashPassword(account, accountDto.Password);

        // TODO: 아래 두 처리는 트랜잭션으로 묶어야 함.
        var accountId = await _accountRepository.CreateAccountAsync(account);

        await _playerRepository.CreatePlayerAsync(new PlayerEntity
        {
            AccountId = accountId,
            PlayerLevel = 1,
            GoldLevel = 1,
            Stage = 1,
            Gold = 0
        });

        return response;
    }

    public async Task<ApiResponse<AccountDto>> LoginAsync(AccountDto accountDto)
    {
        Console.WriteLine($"[{nameof(AuthService)}.{nameof(LoginAsync)}] Email: {accountDto.Email}, Password: {accountDto.Password}");

        var response = new ApiResponse<AccountDto>();

        var account = await _accountRepository.GetAccountAsync(accountDto.Email);
        if (account is null)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.AccountNotExist;
            return response;
        }

        var verify = _passwordHasher.VerifyHashedPassword(account, account.Password, accountDto.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.WrongPassword;
            return response;
        }

        // 기존 접속 토큰이 있는지 확인
        var existingToken = await _accountStore.GetSessionTokenByAccountIdAsync(account.AccountId);

        // 세션 생성 시도
        string? newToken = null;

        // 동일한 토큰이 존재하는 상황을 대비한 코드 (불필요한 코드일 수 있음)
        for (var i = 0; i < 5; i++)
        {
            var token = TokenGenerator.CreateSessionToken();

            var session = new UserSession
            {
                AccountId = account.AccountId,
                Email = account.Email,
                Token = token,
                LoginTime = DateTime.UtcNow
            };

            if (await _accountStore.TryCreateSessionAsync(session, TimeSpan.FromMinutes(5)))
            {
                newToken = token;
                break;
            }
        }

        if (newToken is null)
        {
            response.Ok = false;
            response.ErrorCode = ErrorCode.InternalServerError;
            return response;
        }
                
        if (existingToken is not null)
        {
            // TODO: 기존 세션이 있을 경우 처리 (어떤 처리를 해야할까?)
        }

        await _accountRepository.LoginAsync(account.AccountId);
                
        response.Result = new AccountDto
        {
            Email = accountDto.Email,
            Password = "",
            Token = newToken
        };

        Console.WriteLine($"[{nameof(AuthService)}.{nameof(LoginAsync)}] Success Email: {accountDto.Email}, AccountId: {account.AccountId}, Token: {accountDto.Token}");

        return response;
    }
}
