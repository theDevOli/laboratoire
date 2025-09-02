using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolGetterToDisplayService
(
    IProtocolRepository protocolRepository,
    ICropsNormalizationGetterService cropsNormalizationGetterService,
    ICropGetterService cropGetterService,
    ILogger<ProtocolGetterToDisplayService> logger
)
: IProtocolGetterToDisplayService
{
    public async Task<IEnumerable<ProtocolDtoDisplay>?> GetDisplayProtocolsAsync(int year, Guid? id, bool? isPartner)
    {
        logger.LogInformation("Starting to fetch protocols to display for year {Year}.", year);

        var cropsNormalizationsTask = cropsNormalizationGetterService.GetAllCropsAsync();
        //FIXME:Remove filter
        // var protocolsTask = protocolRepository.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year);
        var protocolsTask = protocolRepository.GetDisplayProtocolsAsync<ProtocolDtoDisplayDb>(year, !isPartner ?? true);
    
        var cropsTask = cropGetterService.GetAllCropsAsync();

        logger.LogDebug("Fetching crops normalizations, protocols, and crops in parallel.");
        await Task.WhenAll(cropsNormalizationsTask, protocolsTask, cropsTask);

        var cropsNormalizations = await cropsNormalizationsTask;
        var protocols = await protocolsTask;
        var crops = await cropsTask;

        var displayProtocols = protocols.ToProtocolDisplay(cropsNormalizations, crops);
        if (id is null && isPartner is null)
        {
            logger.LogInformation("No filtering by ID or role (partner/client). Returning all display protocols.");
            return displayProtocols;
        }

        if ((bool)isPartner!)
        {
            logger.LogInformation("Filtering protocols by Partner ID: {Id}", id);
            return displayProtocols.Where(protocol => protocol.PartnerId == id);
        }

        logger.LogInformation("Filtering protocols by Client ID: {Id}", id);
        return displayProtocols.Where(protocol => protocol.ClientId == id);
    }
}
