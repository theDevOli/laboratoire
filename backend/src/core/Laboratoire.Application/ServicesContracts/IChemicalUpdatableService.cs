using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalUpdatableService
{
    Task<Error> UpdateChemicalAsync(ChemicalDtoGetUpdate chemicalDto);
}
