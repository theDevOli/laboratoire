using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Laboratoire.Application.DTO;
using Laboratoire.Application.IUtils;
using Laboratoire.Application.Services.AuthServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace Laboratoire.Test.Services.AuthServices
{
    public class AuthTokenValidatorServiceTest
    {
        private readonly Mock<ILogger<AuthTokenValidatorService>> _loggerMock;
        private readonly Mock<IToken> _tokenMock;
        private readonly AuthTokenValidatorService _service;

        public AuthTokenValidatorServiceTest()
        {
            _loggerMock = new Mock<ILogger<AuthTokenValidatorService>>();
            _tokenMock = new Mock<IToken>();
            _service = new AuthTokenValidatorService(_loggerMock.Object, _tokenMock.Object);
        }

        [Fact]
        public void ValidateTokenAsync_ShouldReturnFalse_WhenTokenIsNullOrEmpty()
        {
            var dto = new AuthDtoToken { Token = null };

            var result = _service.ValidateTokenAsync(dto);

            Assert.False(result);
        }

        [Fact]
        public void ValidateTokenAsync_ShouldReturnFalse_WhenTokenMissingClaims()
        {
            var tokenString = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken());
            var dto = new AuthDtoToken { Token = tokenString };

            var result = _service.ValidateTokenAsync(dto);

            Assert.False(result);
        }

        [Fact]
        public void ValidateTokenAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var claims = new[]
            {
                new Claim("userId", Guid.NewGuid().ToString()),
                new Claim("role", "Admin")
            };
            var jwtToken = new JwtSecurityToken(claims: claims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var dto = new AuthDtoToken { Token = tokenString };

            _tokenMock.Setup(t => t.ValidateToken(tokenString)).Returns(true);

            // Act
            var result = _service.ValidateTokenAsync(dto);

            // Assert
            Assert.True(result);
            _tokenMock.Verify(t => t.ValidateToken(tokenString), Times.Once);
        }

        [Fact]
        public void ValidateTokenAsync_ShouldReturnFalse_WhenTokenIsInvalid()
        {
            var claims = new[]
            {
                new Claim("userId", Guid.NewGuid().ToString()),
                new Claim("role", "Admin")
            };
            var jwtToken = new JwtSecurityToken(claims: claims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var invalidToken = tokenString + "corruption";

            var dto = new AuthDtoToken { Token = invalidToken };

            _tokenMock.Setup(t => t.ValidateToken(invalidToken)).Returns(false);

            var result = _service.ValidateTokenAsync(dto);

            Assert.False(result);
            _tokenMock.Verify(t => t.ValidateToken(invalidToken), Times.Once);
        }
    }
}
