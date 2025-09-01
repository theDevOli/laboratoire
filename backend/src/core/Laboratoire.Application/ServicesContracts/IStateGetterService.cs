using System;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IStateGetterService
{
    Task<IEnumerable<State>?> GetAllStatesAsync();
}
