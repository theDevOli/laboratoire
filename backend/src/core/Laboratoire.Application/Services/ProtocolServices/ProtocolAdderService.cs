using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolAdderService
(
 IProtocolRepository protocolRepository,
 ICropsNormalizationAdderService cropsNormalizationAdderService,
 ICashFlowAdderService cashFlowAdderService,
 ILogger<ProtocolAdderService> logger
)
: IProtocolAdderService
{
    public async Task<Error> AddProtocolAsync(ProtocolDtoAdd protocolDto)
    {
        logger.LogInformation("Starting protocol addition process.");
        var protocol = protocolDto.ToProtocol();
        logger.LogDebug("Converted DTO to Protocol entity: {@Protocol}", protocol);

        var exists = await protocolRepository.DoesProtocolExistByUniqueAsync(protocol);
        if (exists)
        {
            logger.LogWarning("Conflict detected: a protocol with the same unique fields already exists.");
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var protocolId = await protocolRepository.AddProtocolAsync(protocol);
        logger.LogInformation("Protocol successfully added with ID: {ProtocolId}", protocolId);

        if (protocolDto.TotalPaid is not null)
        {
            protocol.ProtocolId = protocolId;
            var cashFlow = protocolDto.ToCashFlow(protocolId!);
            logger.LogDebug("Generated CashFlow from DTO: {@CashFlow}", cashFlow);

            var error = await cashFlowAdderService.AddCashFlowAsync(cashFlow, protocol);

            if (error.IsNotSuccess())
            {
                logger.LogError("Failed to add CashFlow: {Error}", error.Message);
                return error;
            }

            logger.LogInformation("CashFlow successfully added for protocol ID: {ProtocolId}", protocolId);
        }

        if (protocolDto.Crops is null)
        {
            logger.LogInformation("No crops provided in DTO. Ending process successfully.");
            return Error.SetSuccess();
        }

        var cropsNormalization = protocolDto.ToCropsNormalization(protocolId!);
        if (cropsNormalization is null)
        {
            logger.LogInformation("Crops normalization conversion returned null. Ending process successfully.");
            return Error.SetSuccess();
        }

        var addError = await cropsNormalizationAdderService.AddCropsAsync(cropsNormalization, protocolId!);
        if (addError.IsNotSuccess())
        {
            logger.LogError("Failed to add crops normalization: {Error}", addError.Message);
            return addError;
        }

        logger.LogInformation("CropsNormalization successfully added for protocol ID: {ProtocolId}", protocolId);
        logger.LogInformation("Protocol addition process completed successfully.");
        return Error.SetSuccess();
    }
}
