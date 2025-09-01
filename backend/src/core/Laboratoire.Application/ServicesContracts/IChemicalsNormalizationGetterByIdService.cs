using System;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalsNormalizationGetterByIdService
{
    Task<IEnumerable<ChemicalsNormalization>?> GetHazardsByIdAsync(int? chemicalId);
}
