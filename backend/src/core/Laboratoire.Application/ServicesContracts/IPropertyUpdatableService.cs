using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPropertyUpdatableService
{
    Task<Error> UpdatePropertyAsync(Property property);
}
