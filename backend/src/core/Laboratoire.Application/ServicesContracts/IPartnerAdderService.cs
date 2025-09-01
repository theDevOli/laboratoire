using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IPartnerAdderService
{
    Task<Error> AddPartnerAsync(PartnerDtoAdd partner);
}
