using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Laboratoire.Application.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Laboratoire.Application.Utils;

public class Token
(
    IConfiguration config
)
{
    // private readonly IConfiguration _config = config;
    private readonly SymmetricSecurityKey _tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config.GetSection("Appsettings:TokenKey").Value!)
        );
    public string CreateToken(Guid? userId, string? role)
    {
        Claim[] claims = new Claim[]{
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId",userId.ToString()!),
            new Claim("role",role!)
        };

        // SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        //     config.GetSection("Appsettings:TokenKey").Value!)
        // );

        SigningCredentials credentials = new SigningCredentials(
            _tokenKey,
            SecurityAlgorithms.HmacSha512Signature
        );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(Constants.TOKEN_DURATION)
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }

    public string RefreshToken(AuthDtoRefreshToken authDto)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtTokenHandler.ReadJwtToken(authDto.RefreshToken);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId");
        var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");

        var userId = Guid.Parse(userIdClaim?.Value!);
        var role = roleClaim?.Value;

        return CreateToken(userId, role);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _tokenKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

}
