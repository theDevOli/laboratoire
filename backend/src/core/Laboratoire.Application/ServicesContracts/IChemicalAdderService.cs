using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalAdderService
{
    Task<Error> AddChemicalAsync(ChemicalDtoAdd chemicalDto);
}
