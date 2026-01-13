using GameServer.Domain.DTOs;
using GameServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using GameServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using GameServer.Domain.Models;
using GameServer.Controllers;

namespace UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAccountRepository> _repo;
    private readonly Mock<IAccountCache> _cache;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _repo = new Mock<IAccountRepository>();
        _cache = new Mock<IAccountCache>();
        _controller = new AuthController(_repo.Object, _cache.Object);
    }


    [Fact()]
    public async Task Register_WhenAccountExists_ReturnsConflict()
    {
        _repo.Setup(r => r.ExistsAsync("test@mail.com"))
            .ReturnsAsync(true);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _controller.Register(dto);

        result.Should().BeOfType<ConflictResult>();

        _repo.Verify(r => r.CreateAccountAsync(It.IsAny<AccountEntity>()), Times.Never);
    }

    [Fact]
    public async Task Register_WhenNewAccount_ReturnsOk()
    {
        _repo.Setup(r => r.ExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _controller.Register(dto);

        result.Should().BeOfType<OkResult>();

        _repo.Verify(r => r.CreateAccountAsync(
            It.Is<AccountEntity>(a =>
                a.Email == "test@mail.com" &&
                !string.IsNullOrEmpty(a.Password)
            )
        ), Times.Once);
    }

    [Fact]
    public async Task Login_WhenAccountNotExists_ReturnsUnauthorized()
    {
        _repo.Setup(r => r.GetAccountAsync("test@mail.com"))
            .ReturnsAsync((AccountEntity?)null);

        var dto = new AccountDto
        {
            Email = "test@mail.com",
            Password = "test1234"
        };

        var result = await _controller.Login(dto);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact()]
    public async Task Login_WhenPasswordWrong_ReturnsBadRequest()
    {
        var hasher = new PasswordHasher<AccountEntity>();
        var account = new AccountEntity
        {
            AccountId = 1,
            Email = "test@mail.com",
            Password = hasher.HashPassword(new AccountEntity(), "test1234")
        };

        _repo.Setup(r => r.GetAccountAsync(account.Email))
            .ReturnsAsync(account);

        var dto = new AccountDto
        {
            Email = account.Email,
            Password = "test"
        };

        var result = await _controller.Login(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_WhenSuccess_ReturnsOkWithToken()
    {
        var hasher = new PasswordHasher<AccountEntity>();
        var account = new AccountEntity
        {
            AccountId = 1,
            Email = "test@mail.com",
            Password = hasher.HashPassword(new AccountEntity(), "test1234")
        };

        _repo.Setup(r => r.GetAccountAsync(account.Email))
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

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<AccountDto>().Subject;

        body.Token.Should().NotBeNullOrEmpty();
        body.Password.Should().BeEmpty();

        _repo.Verify(r => r.LoginAsync(account.AccountId), Times.Once);
    }
}