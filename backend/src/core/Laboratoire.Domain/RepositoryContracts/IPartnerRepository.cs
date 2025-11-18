using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IPartnerRepository
{
    Task<IEnumerable<Partner>> GetAllPartnersAsync();
    Task<IEnumerable<Partner>> GetActivePartnersAsync();
    Task<Partner?> GetPartnerByIdAsync(Guid? partnerId);
    Task<Partner?> GetPartnerByOfficeAndNameAsync(Partner partner);
    Task<bool> DoesPartnerExistByIdAsync(Partner partner);
    Task<bool> DoesPartnerExistByOfficeAndNameAsync(Partner partner);
    // Task<bool> AddPartnerAsync(Partner partner, Guid? userId);
    Task<bool> UpdatePartnerAsync(Partner partner);

}
