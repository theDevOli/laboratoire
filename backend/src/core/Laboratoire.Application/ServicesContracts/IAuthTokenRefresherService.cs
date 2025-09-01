using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthTokenRefresherService
{
    Task<string?> RefreshToken(AuthDtoRefreshToken authDto);
}
