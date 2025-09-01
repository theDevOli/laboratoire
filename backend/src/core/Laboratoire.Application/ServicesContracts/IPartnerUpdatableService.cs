using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IPartnerUpdatableService
{
    Task<Error> UpdatePartnerAsync(Partner partner);
}
