using System.IdentityModel.Tokens.Jwt;
using Laboratoire.Application.DTO;
using Laboratoire.Application.IUtils;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.AuthServices;

public class AuthTokenRefresherService
(
    ILogger<AuthTokenRefresherService> logger,
    IToken token
)
: IAuthTokenRefresherService
{
    public string? RefreshToken(AuthDtoRefreshToken authDto)
    {
        logger.LogInformation("Refreshing token is initialized.");
        if (string.IsNullOrEmpty(authDto.RefreshToken))
        {
            logger.LogError("Refresh token is null or empty.");
            return null;
        }

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtTokenHandler.ReadJwtToken(authDto.RefreshToken);

        if (jwtToken is null)
        {
            logger.LogError("Failed to parse refresh token.");
            return null;
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId");
        var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");
        if (userIdClaim is null || roleClaim is null)
        {
            logger.LogWarning("userId claim not found in the refresh token.");
            return null;
        }

        logger.LogInformation("Refreshing token is successfully completed.");
        return token.RefreshToken(authDto);
    }
}
