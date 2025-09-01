using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPartnerGetterByIdService
{
    Task<Partner?> GetPartnerByIdAsync(Guid? partnerId);
}
