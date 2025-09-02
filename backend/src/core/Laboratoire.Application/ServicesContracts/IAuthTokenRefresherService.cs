using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthTokenRefresherService
{
    string? RefreshToken(AuthDtoRefreshToken authDto);
}
