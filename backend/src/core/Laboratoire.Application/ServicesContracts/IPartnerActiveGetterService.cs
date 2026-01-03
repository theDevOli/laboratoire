using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPartnerActiveGetterService
{
    public Task<IEnumerable<Partner>> GetActivePartnersAsync();
}
