
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Laboratoire.Infrastructure.Extensions;

public static class JwtExtension
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenKey = configuration["Appsettings:TokenKey"];
        if (string.IsNullOrEmpty(tokenKey) || tokenKey.Length < 64)
        {
            throw new InvalidOperationException("Token key must be at least 64 characters long.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
          IssuerSigningKey = key,
            ValidateIssuer = false,
            // ValidIssuer = false,
            ValidateAudience = false,
            // ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
                };
            });
    }
}
