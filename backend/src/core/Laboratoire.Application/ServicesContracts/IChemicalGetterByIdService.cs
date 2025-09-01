using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IChemicalGetterByIdService
{
    Task<ChemicalDtoGetUpdate?> GetChemicalByIdAsync(int? chemicalId);
}
