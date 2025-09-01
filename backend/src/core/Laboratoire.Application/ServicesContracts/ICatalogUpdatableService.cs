using System;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICatalogUpdatableService
{
    Task<Error> UpdateCatalogAsync(Catalog catalog);
}
