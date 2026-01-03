
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;

namespace Laboratoire.Application.Services.PartnerServices;

public class PartnerActiveGetterService
(
    IPartnerRepository partnerRepository
)
: IPartnerActiveGetterService
{
    public async Task<IEnumerable<Partner>> GetActivePartnersAsync()
    => await partnerRepository.GetActivePartnersAsync();
}
