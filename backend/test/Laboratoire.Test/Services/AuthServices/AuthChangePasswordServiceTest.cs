using System.Security.Cryptography;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Services.AuthServices;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Tests.Services.AuthServices;

public class AuthChangePasswordServiceTest
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly IConfiguration _config;
    private readonly PasswordHasher _passwordHasher;
    private readonly Mock<ILogger<AuthChangePasswordService>> _loggerMock;
    private readonly AuthChangePasswordService _service;

    public AuthChangePasswordServiceTest()
    {
        var inMemorySettings = new Dictionary<string, string> {
        {"AppSettings:PasswordKey", "SuperSecretKey!"}
    };
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        _authRepositoryMock = new Mock<IAuthRepository>();
        _passwordHasher = new PasswordHasher(_config);
        _loggerMock = new Mock<ILogger<AuthChangePasswordService>>();
        _service = new AuthChangePasswordService(
            _authRepositoryMock.Object,
            _passwordHasher,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userDto = new UserDtoChangePassword
        {
            UserId = Guid.NewGuid(),
            OldPassword = "123",
            UserPassword = "456"
        };

        _authRepositoryMock
            .Setup(r => r.GetAuthByUserIdAsync(userDto.UserId))
            .ReturnsAsync((Auth?)null);

        // Act
        var result = await _service.ChangeUserPasswordAsync(userDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.NotNull(result.Message);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldReturnUnauthorized_WhenOldPasswordIsWrong()
    {
        // Arrange
        var userDto = new UserDtoChangePassword
        {
            UserId = Guid.NewGuid(),
            OldPassword = "wrong-old",
            UserPassword = "newPass"
        };

        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        var correctHash = _passwordHasher.HashPassword("correct-old", salt);

        var auth = new Auth
        {
            UserId = userDto.UserId,
            PasswordSalt = salt,
            PasswordHash = correctHash
        };

        _authRepositoryMock
            .Setup(r => r.GetAuthByUserIdAsync(userDto.UserId))
            .ReturnsAsync(auth);

        // Act
        var result = await _service.ChangeUserPasswordAsync(userDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.NotNull(result.Message);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldReturnDbError_WhenUpdateFails()
    {
        // Arrange
        var userDto = new UserDtoChangePassword
        {
            UserId = Guid.NewGuid(),
            OldPassword = "correct-old",
            UserPassword = "newPass"
        };

        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        var hash = _passwordHasher.HashPassword(userDto.OldPassword, salt);

        var auth = new Auth
        {
            UserId = userDto.UserId,
            PasswordSalt = salt,
            PasswordHash = hash
        };

        _authRepositoryMock
            .Setup(r => r.GetAuthByUserIdAsync(userDto.UserId))
            .ReturnsAsync(auth);

        _authRepositoryMock
            .Setup(r => r.UpdateAuthAsync(It.IsAny<Auth>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.ChangeUserPasswordAsync(userDto);

        // Assert
        Assert.True(result.IsNotSuccess());
        Assert.NotNull(result.Message);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldReturnSuccess_WhenPasswordChanged()
    {
        // Arrange
        var userDto = new UserDtoChangePassword
        {
            UserId = Guid.NewGuid(),
            OldPassword = "correct-old",
            UserPassword = "newPass"
        };

        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        var hash = _passwordHasher.HashPassword(userDto.OldPassword, salt);

        var auth = new Auth
        {
            UserId = userDto.UserId,
            PasswordSalt = salt,
            PasswordHash = hash
        };

        _authRepositoryMock
            .Setup(r => r.GetAuthByUserIdAsync(userDto.UserId))
            .ReturnsAsync(auth);

        _authRepositoryMock
            .Setup(r => r.UpdateAuthAsync(It.IsAny<Auth>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ChangeUserPasswordAsync(userDto);

        // Assert
        Assert.False(result.IsNotSuccess());
        Assert.Null(result.Message);
        Assert.Equal(0, result.StatusCode);
    }
}
