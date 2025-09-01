using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPartnerGetterService
{
    Task<IEnumerable<Partner>> GetAllPartnersAsync();
}
