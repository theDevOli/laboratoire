using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPropertyAdderService
{
    Task<Error> AddPropertyAsync(PropertyDtoAdd property);
}
