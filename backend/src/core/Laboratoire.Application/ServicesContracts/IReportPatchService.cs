using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IReportPatchService
{
    Task<Error> PatchReportAsync(Report report);
}
