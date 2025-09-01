using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IChemicalRepository
{
    Task<IEnumerable<Chemical>> GetAllChemicalsAsync();
    Task<Chemical?> GetChemicalByIdAsync(int? chemicalId);
    Task<Chemical?> GetChemicalByNameAndConcentrationAsync(Chemical chemical);
    Task<bool> DoesChemicalExistByIdAsync(Chemical chemical);
    Task<bool> DoesChemicalExistByNameAndConcentrationAsync(Chemical chemical);
    Task<int?> AddChemicalAsync(Chemical chemical);
    Task<bool> UpdateChemicalAsync(Chemical chemical);
}
