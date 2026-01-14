using GameServer.Repositories.Interfaces;
using Moq;
using Microsoft.AspNetCore.Identity;
using GameServer.Controllers;
using GameServer.Models.DbModels;
using GameServer.Models.DTOs;

namespace UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IPlayerRepository> _playerRepository;
    private readonly Mock<IAccountRepository> _accountRepository;
    private readonly Mock<IAccountCache> _accountCache;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _playerRepository = new Mock<IPlayerRepository>();
        _accountRepository = new Mock<IAccountRepository>();
        _accountCache = new Mock<IAccountCache>();
        _controller = new AuthController(_playerRepository.Object, _accountRepository.Object, _accountCache.Object);
    }


    [Fact()]
    public async Task Register_WhenEmailExists_ShouldReturnEmailAlreadyExists()
    {
        _accountRepository.Setup(r => r.ExistsAsync("test@mail.com"))
            .ReturnsAsync(true);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _controller.Register(dto);
        var response = result.ExtractOkValue<ApiResponse>();

        Assert.False(response.Ok);
        Assert.Equal(ErrorCode.EmailAlreadyExists, response.ErrorCode);
        Assert.Equal(nameof(ErrorCode.EmailAlreadyExists), response.ErrorCodeName);

        _accountRepository.Verify(r => r.CreateAccountAsync(It.IsAny<Account>()), Times.Never);

        _accountRepository.Verify(r => r.ExistsAsync(dto.Email), Times.Once);
        _accountRepository.Verify(r => r.CreateAccountAsync(It.IsAny<Account>()), Times.Never);
        _playerRepository.Verify(r => r.CreatePlayerAsync(It.IsAny<PlayerInfo>()), Times.Never);
    }

    [Fact()]
    public async Task Register_WhenNewEmail_ShouldCreateAccount_AndReturnOkResult()
    {
        const long accountId = 1000;
        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        _accountRepository.Setup(r => r.ExistsAsync(dto.Email))
                   .ReturnsAsync(false);

        _accountRepository.Setup(r => r.CreateAccountAsync(It.Is<Account>(a =>
                        a.Email == dto.Email &&
                        !string.IsNullOrWhiteSpace(a.Password)
                        )))
                   .ReturnsAsync(accountId);

        var playerInfo = new PlayerInfo();
        _playerRepository.Setup(r => r.CreatePlayerAsync(playerInfo))
                  .Returns(Task.CompletedTask);

        var result = await _controller.Register(dto);
        var response = result.ExtractOkValue<ApiResponse>();

        Assert.True(response.Ok);
        Assert.Equal(ErrorCode.Ok, response.ErrorCode);
        Assert.Equal(nameof(ErrorCode.Ok), response.ErrorCodeName);

        _accountRepository.Verify(r => r.ExistsAsync(dto.Email), Times.Once);
        _accountRepository.Verify(r => r.CreateAccountAsync(
            It.Is<Account>(a =>
                a.Email == "test@mail.com" &&
                !string.IsNullOrEmpty(a.Password)
            )
        ), Times.Once);
        _playerRepository.Verify(r => r.CreatePlayerAsync(It.IsAny<PlayerInfo>()), Times.Once);
    }

    [Fact()]
    public async Task Login_WhenAccountNotExist_ShouldReturnAccountNotExist()
    {
        _accountRepository.Setup(r => r.GetAccountAsync("test@mail.com"))
            .ReturnsAsync((Account?)null);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _controller.Login(dto);

        var response = result.ExtractOkValue<ApiResponse<AccountDto>>();
        Assert.False(response.Ok);
        Assert.Equal(ErrorCode.AccountNotExist, response.ErrorCode);

        _accountRepository.Verify(r => r.LoginAsync(It.IsAny<long>()), Times.Never);
        _accountCache.Verify(c => c.TryCreateSessionAsync(It.IsAny<UserSession>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact()]
    public async Task Login_WhenWrongPassword_ShouldReturnWrongPassword()
    {
        var hasher = new PasswordHasher<Account>();
        var account = new Account
        {
            AccountId = 1,
            Email = "test@mail.com",
            Password = hasher.HashPassword(new Account(), "test1234")
        };

        _accountRepository.Setup(r => r.GetAccountAsync(account.Email))
            .ReturnsAsync(account);

        var dto = new AccountDto
        {
            Email = account.Email,
            Password = "test"
        };

        var result = await _controller.Login(dto);

        var response = result.ExtractOkValue<ApiResponse<AccountDto>>();
        Assert.False(response.Ok);
        Assert.Equal(ErrorCode.WrongPassword, response.ErrorCode);

        _accountRepository.Verify(r => r.LoginAsync(It.IsAny<long>()), Times.Never);
        _accountCache.Verify(c => c.TryCreateSessionAsync(It.IsAny<UserSession>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact()]
    public async Task Login_WhenSuccess_ShouldReturnToken_AndCallLoginAsync()
    {
        var hasher = new PasswordHasher<Account>();
        var account = new Account
        {
            AccountId = 1000,
            Email = "test@mail.com",
            Password = hasher.HashPassword(new Account(), "test1234")
        };

        _accountRepository.Setup(r => r.GetAccountAsync(account.Email))
            .ReturnsAsync(account);

        _accountCache.Setup(c => c.GetSessionTokenByAccountIdAsync(account.AccountId))
            .ReturnsAsync((string?)null);

        _accountCache.Setup(c => c.TryCreateSessionAsync(
                It.IsAny<UserSession>(),
                It.IsAny<TimeSpan>()
            ))
            .ReturnsAsync(true);

        var dto = new AccountDto
        {
            Email = account.Email,
            Password = "test1234"
        };

        var result = await _controller.Login(dto);

        var response = result.ExtractOkValue<ApiResponse<AccountDto>>();
        Assert.True(response.Ok);
        Assert.NotNull(response.Result);
        Assert.Equal("test@mail.com", response.Result!.Email);
        Assert.Equal("", response.Result.Password);
        Assert.False(string.IsNullOrWhiteSpace(response.Result.Token));

        _accountRepository.Verify(r => r.LoginAsync(1000), Times.Once);
        _accountCache.Verify(c => c.TryCreateSessionAsync(
            It.Is<UserSession>(s => s.AccountId == 1000 && s.Email == "test@mail.com" && !string.IsNullOrWhiteSpace(s.Token)),
            It.Is<TimeSpan>(t => t == TimeSpan.FromMinutes(5))
        ), Times.Once);
    }
}