using System;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IOfficeGetterByIdService
{
    public Task<Office?> GetByOfficeIdAsync(Guid? officeId);
}
