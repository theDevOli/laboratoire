using System;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IUserDeletionService
{
    Task<Error> DeletionUserAsync(User user);
}
