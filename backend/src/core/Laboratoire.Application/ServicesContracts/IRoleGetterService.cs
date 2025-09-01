using System;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IRoleGetterService
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
}
