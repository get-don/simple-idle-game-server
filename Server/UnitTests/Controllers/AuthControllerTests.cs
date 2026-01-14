using GameServer.Repositories.Interfaces;
using Moq;
using Microsoft.AspNetCore.Identity;
using GameServer.Controllers;
using GameServer.Models.DbModels;
using GameServer.Models.DTOs;

namespace UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAccountRepository> _repository;
    private readonly Mock<IAccountCache> _cache;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _repository = new Mock<IAccountRepository>();
        _cache = new Mock<IAccountCache>();
        _controller = new AuthController(_repository.Object, _cache.Object);
    }


    [Fact()]
    public async Task Register_WhenEmailExists_ShouldReturnEmailAlreadyExists()
    {
        _repository.Setup(r => r.ExistsAsync("test@mail.com"))
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

        _repository.Verify(r => r.CreateAccountAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact()]
    public async Task Register_WhenNewEmail_ShouldCreateAccount_AndReturnOkResult()
    {
        _repository.Setup(r => r.ExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _controller.Register(dto);
        var response = result.ExtractOkValue<ApiResponse>();

        Assert.True(response.Ok);

        _repository.Verify(r => r.CreateAccountAsync(
            It.Is<Account>(a =>
                a.Email == "test@mail.com" &&
                !string.IsNullOrEmpty(a.Password)
            )
        ), Times.Once);
    }

    [Fact()]
    public async Task Login_WhenAccountNotExist_ShouldReturnAccountNotExist()
    {
        _repository.Setup(r => r.GetAccountAsync("test@mail.com"))
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

        _repository.Verify(r => r.LoginAsync(It.IsAny<long>()), Times.Never);
        _cache.Verify(c => c.TryCreateSessionAsync(It.IsAny<UserSession>(), It.IsAny<TimeSpan>()), Times.Never);
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

        _repository.Setup(r => r.GetAccountAsync(account.Email))
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

        _repository.Verify(r => r.LoginAsync(It.IsAny<long>()), Times.Never);
        _cache.Verify(c => c.TryCreateSessionAsync(It.IsAny<UserSession>(), It.IsAny<TimeSpan>()), Times.Never);
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

        _repository.Setup(r => r.GetAccountAsync(account.Email))
            .ReturnsAsync(account);

        _cache.Setup(c => c.GetSessionTokenByAccountIdAsync(account.AccountId))
            .ReturnsAsync((string?)null);

        _cache.Setup(c => c.TryCreateSessionAsync(
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

        _repository.Verify(r => r.LoginAsync(1000), Times.Once);
        _cache.Verify(c => c.TryCreateSessionAsync(
            It.Is<UserSession>(s => s.AccountId == 1000 && s.Email == "test@mail.com" && !string.IsNullOrWhiteSpace(s.Token)),
            It.Is<TimeSpan>(t => t == TimeSpan.FromMinutes(5))
        ), Times.Once);
    }
}