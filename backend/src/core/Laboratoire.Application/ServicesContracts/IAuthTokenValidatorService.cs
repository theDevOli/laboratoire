using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IAuthTokenValidatorService
{
    bool ValidateTokenAsync(AuthDtoToken token);
}
