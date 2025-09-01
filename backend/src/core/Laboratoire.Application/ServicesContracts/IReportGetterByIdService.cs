using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IReportGetterByIdService
{
    Task<Report?> GetReportByIdAsync(Guid? reportId);

}
