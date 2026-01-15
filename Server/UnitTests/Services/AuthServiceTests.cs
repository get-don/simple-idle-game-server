using Moq;
using GameServer.Repositories.Interfaces;
using GameServer.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using GameServer.Models.DTOs;
using GameServer.Models;

namespace GameServer.Services.Tests;

public class AuthServiceTests
{
    private readonly Mock<IPlayerRepository> _playerRepository;
    private readonly Mock<IAccountRepository> _accountRepository;
    private readonly Mock<IAccountStore> _accountStore;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _playerRepository = new Mock<IPlayerRepository>();
        _accountRepository = new Mock<IAccountRepository>();
        _accountStore = new Mock<IAccountStore>();

        var passwordHasher = new PasswordHasher<AccountEntity>();

        _authService = new AuthService(_playerRepository.Object, _accountRepository.Object, _accountStore.Object, passwordHasher);
    }


    [Fact()]
    public async Task RegisterAsync_WhenEmailExists_ShouldReturnEmailAlreadyExists()
    {
        _accountRepository.Setup(r => r.ExistsAsync("test@mail.com"))
            .ReturnsAsync(true);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.NotNull(result);
        Assert.False(result.Ok);
        Assert.Equal(ErrorCode.EmailAlreadyExists, result.ErrorCode);
        Assert.Equal(nameof(ErrorCode.EmailAlreadyExists), result.ErrorCodeName);

        _accountRepository.Verify(r => r.CreateAccountAsync(It.IsAny<AccountEntity>()), Times.Never);
        _accountRepository.Verify(r => r.ExistsAsync(dto.Email), Times.Once);
        _accountRepository.Verify(r => r.CreateAccountAsync(It.IsAny<AccountEntity>()), Times.Never);
        _playerRepository.Verify(r => r.CreatePlayerAsync(It.IsAny<PlayerEntity>()), Times.Never);
    }

    [Fact()]
    public async Task RegisterAsync_WhenNewEmail_ShouldCreateAccount_AndReturnOkResult()
    {
        const long accountId = 1000;
        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        _accountRepository.Setup(r => r.ExistsAsync(dto.Email))
                   .ReturnsAsync(false);

        _accountRepository.Setup(r => r.CreateAccountAsync(It.Is<AccountEntity>(a =>
                        a.Email == dto.Email &&
                        !string.IsNullOrWhiteSpace(a.Password)
                        )))
                   .ReturnsAsync(accountId);

        var player = new PlayerEntity();
        _playerRepository.Setup(r => r.CreatePlayerAsync(player))
                  .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Ok);
        Assert.Equal(ErrorCode.Ok, result.ErrorCode);
        Assert.Equal(nameof(ErrorCode.Ok), result.ErrorCodeName);

        _accountRepository.Verify(r => r.ExistsAsync(dto.Email), Times.Once);
        _accountRepository.Verify(r => r.CreateAccountAsync(
            It.Is<AccountEntity>(a =>
                a.Email == "test@mail.com" &&
                !string.IsNullOrEmpty(a.Password)
            )
        ), Times.Once);
        _playerRepository.Verify(r => r.CreatePlayerAsync(It.IsAny<PlayerEntity>()), Times.Once);
    }

    [Fact()]
    public async Task LoginAsync_WhenAccountNotExist_ShouldReturnAccountNotExist()
    {
        _accountRepository.Setup(r => r.GetAccountAsync("test@mail.com"))
            .ReturnsAsync((AccountEntity?)null);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.False(result.Ok);
        Assert.Equal(ErrorCode.AccountNotExist, result.ErrorCode);

        _accountRepository.Verify(r => r.LoginAsync(It.IsAny<long>()), Times.Never);
        _accountStore.Verify(c => c.TryCreateSessionAsync(It.IsAny<UserSession>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact()]
    public async Task LoginAsync_WhenWrongPassword_ShouldReturnWrongPassword()
    {
        var hasher = new PasswordHasher<AccountEntity>();
        var account = new AccountEntity
        {
            AccountId = 1,
            Email = "test@mail.com",
            Password = hasher.HashPassword(new AccountEntity(), "test1234")
        };

        _accountRepository.Setup(r => r.GetAccountAsync(account.Email))
            .ReturnsAsync(account);

        var dto = new AccountDto
        {
            Email = account.Email,
            Password = "test"
        };

        var result = await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.False(result.Ok);
        Assert.Equal(ErrorCode.WrongPassword, result.ErrorCode);

        _accountRepository.Verify(r => r.LoginAsync(It.IsAny<long>()), Times.Never);
        _accountStore.Verify(c => c.TryCreateSessionAsync(It.IsAny<UserSession>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact()]
    public async Task LoginAsync_WhenSuccess_ShouldReturnToken_AndCallLoginAsync()
    {
        var hasher = new PasswordHasher<AccountEntity>();
        var account = new AccountEntity
        {
            AccountId = 1000,
            Email = "test@mail.com",
            Password = hasher.HashPassword(new AccountEntity(), "test1234")
        };

        _accountRepository.Setup(r => r.GetAccountAsync(account.Email))
            .ReturnsAsync(account);

        _accountStore.Setup(c => c.GetSessionTokenByAccountIdAsync(account.AccountId))
            .ReturnsAsync((string?)null);

        _accountStore.Setup(c => c.TryCreateSessionAsync(
                It.IsAny<UserSession>(),
                It.IsAny<TimeSpan>()
            ))
            .ReturnsAsync(true);

        var dto = new AccountDto
        {
            Email = account.Email,
            Password = "test1234"
        };

        var result = await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.True(result.Ok);
        Assert.NotNull(result.Result);
        Assert.Equal("test@mail.com", result.Result!.Email);
        Assert.Equal("", result.Result.Password);
        Assert.False(string.IsNullOrWhiteSpace(result.Result.Token));

        _accountRepository.Verify(r => r.LoginAsync(1000), Times.Once);
        _accountStore.Verify(c => c.TryCreateSessionAsync(
            It.Is<UserSession>(s => s.AccountId == 1000 && s.Email == "test@mail.com" && !string.IsNullOrWhiteSpace(s.Token)),
            It.Is<TimeSpan>(t => t == TimeSpan.FromMinutes(5))
        ), Times.Once);
    }
}