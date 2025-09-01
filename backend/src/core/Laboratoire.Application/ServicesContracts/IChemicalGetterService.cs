using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalGetterService
{
    Task<IEnumerable<ChemicalDtoGetUpdate>> GetAllChemicalsAsync();
}
