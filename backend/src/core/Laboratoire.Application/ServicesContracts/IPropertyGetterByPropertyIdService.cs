using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPropertyGetterByPropertyIdService
{
    Task<Property?> GetPropertyByPropertyIdAsync(int? propertyId);
}
