using System;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropGetterService
{
    Task<IEnumerable<Crop>> GetAllCropsAsync();
}
