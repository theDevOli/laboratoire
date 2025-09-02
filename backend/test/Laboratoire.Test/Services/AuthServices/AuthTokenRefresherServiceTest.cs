using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Laboratoire.Application.DTO;
using Laboratoire.Application.IUtils;
using Laboratoire.Application.Services.AuthServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Tests.Services.AuthServices;

public class AuthTokenRefresherServiceTest
{
    private readonly Mock<ILogger<AuthTokenRefresherService>> _loggerMock;
    private readonly Mock<IToken> _tokenMock;
    private readonly AuthTokenRefresherService _service;

    public AuthTokenRefresherServiceTest()
    {
        _tokenMock = new Mock<IToken>();

        _loggerMock = new Mock<ILogger<AuthTokenRefresherService>>();
        _service = new AuthTokenRefresherService(_loggerMock.Object, _tokenMock.Object);
    }

    [Fact]
    public void RefreshToken_ShouldReturnNull_WhenTokenIsNullOrEmpty()
    {
        // Arrange
        var authDto = new AuthDtoRefreshToken { RefreshToken = null };

        // Act
        var result = _service.RefreshToken(authDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RefreshToken_ShouldReturnNull_WhenTokenMissingClaims()
    {
        // Arrange
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tokenString = jwtTokenHandler.WriteToken(new JwtSecurityToken());

        var authDto = new AuthDtoRefreshToken { RefreshToken = tokenString };

        // Act
        var result = _service.RefreshToken(authDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RefreshToken_ShouldReturnNewToken_WhenTokenIsValid()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("userId", Guid.NewGuid().ToString()),
            new Claim("role", "Admin")
        };
        var jwtToken = new JwtSecurityToken(claims: claims);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var authDto = new AuthDtoRefreshToken { RefreshToken = tokenString };
        var expectedNewToken = "new.jwt.token";

        _tokenMock.Setup(t => t.RefreshToken(authDto)).Returns(expectedNewToken);

        // Act
        var result = _service.RefreshToken(authDto);

        // Assert
        Assert.Equal(expectedNewToken, result);
        _tokenMock.Verify(t => t.RefreshToken(authDto), Times.Once);
    }
}
