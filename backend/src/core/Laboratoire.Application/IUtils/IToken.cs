using Laboratoire.Application.DTO;

namespace Laboratoire.Application.IUtils;

public interface IToken
{
    string CreateToken(Guid? userId, string? role);
    string RefreshToken(AuthDtoRefreshToken authDto);
    bool ValidateToken(string token);
}
