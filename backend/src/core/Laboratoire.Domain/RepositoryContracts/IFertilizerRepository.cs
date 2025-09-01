using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IFertilizerRepository
{
    Task<IEnumerable<Fertilizer>> GetAllFertilizersAsync();
    Task<Fertilizer?> GetFertilizerByIdAsync(int? fertilizerId);
    Task<IEnumerable<Fertilizer>> GetFertilizersByProportionAsync(string? proportion);
    Task<bool> ChangeFertilizerStatusAsync(int? fertilizerId);
}
