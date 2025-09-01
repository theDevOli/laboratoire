using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace Laboratoire.Application.Utils;

public class PasswordHasher
(
    IConfiguration config
)
{
    private readonly IConfiguration _config = config;
    public byte[] HashPassword(string password, byte[] passwordSalt)
    {
        var passwordSaltString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 256 / 8
        );

        return passwordHash;

    }
}
