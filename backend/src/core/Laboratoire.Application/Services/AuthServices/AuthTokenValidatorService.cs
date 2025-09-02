using System.IdentityModel.Tokens.Jwt;
using Laboratoire.Application.DTO;
using Laboratoire.Application.IUtils;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.AuthServices;

public class AuthTokenValidatorService
(
    ILogger<AuthTokenValidatorService> logger,
    IToken token
)
: IAuthTokenValidatorService
{
    public bool ValidateTokenAsync(AuthDtoToken dto)
    {
        logger.LogInformation("Refreshing token is initialized.");
        if (string.IsNullOrEmpty(dto.Token))
        {
            logger.LogError("Refresh token is null or empty.");
            return false;
        }

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtTokenHandler.ReadJwtToken(dto.Token);

        if (jwtToken is null)
        {
            logger.LogError("Failed to parse refresh token.");
            return false;
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId");
        var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");
        if (userIdClaim is null || roleClaim is null)
        {
            logger.LogWarning("userId claim not found in the refresh token.");
            return false;
        }

        logger.LogInformation("Refreshing token is successfully completed.");
        return token.ValidateToken(dto.Token);
    }
}
