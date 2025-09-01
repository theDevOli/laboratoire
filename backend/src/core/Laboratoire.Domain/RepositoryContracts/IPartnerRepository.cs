using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IPartnerRepository
{
    Task<IEnumerable<Partner>> GetAllPartnersAsync();
    Task<IEnumerable<Partner>> GetActivePartnersAsync();
    Task<Partner?> GetPartnerByIdAsync(Guid? partnerId);
    Task<Partner?> GetPartnerByEmailAndNameAsync(Partner partner);
    Task<bool> DoesPartnerExistByIdAsync(Partner partner);
    Task<bool> DoesPartnerExistByEmailAndNameAsync(Partner partner);
    Task<bool> AddPartnerAsync(Partner partner, Guid? userId);
    Task<bool> UpdatePartnerAsync(Partner partner);

}
