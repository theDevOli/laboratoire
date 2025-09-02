using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolPatchCashFlowIdService
(
    IProtocolRepository protocolRepository,
    ICashFlowRepository cashFlowRepository,
    ILogger<ProtocolPatchCashFlowIdService> logger
)
: IProtocolPatchCashFlowIdService
{
    public async Task<Error> PatchCashFlowIdAsync(ProtocolDtoUpdateCashFlow protocolDto)
    {
        logger.LogInformation("Starting to patch CashFlowId for protocol ID: {ProtocolId}", protocolDto.ProtocolId);
        var protocol = protocolDto.ToProtocol();

        var exists = await protocolRepository.DoesProtocolExistByProtocolIdAsync(protocol);
        if (!exists)
        {
            logger.LogWarning("Protocol with ID {ProtocolId} not found.", protocol.ProtocolId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await protocolRepository.UpdateCashFlowIdAsync(protocol);
        if (!ok)
        {
            logger.LogError("Failed to update CashFlowId for protocol ID {ProtocolId}.", protocol.ProtocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("CashFlowId updated successfully for protocol ID: {ProtocolId}", protocol.ProtocolId);

        var cashFlow = protocolDto.ToCashFlow();

        var isPatched = await cashFlowRepository.PatchDescriptionAsync(cashFlow);
        if (!isPatched)
        {
            logger.LogError("Failed to patch CashFlow description for CashFlow ID: {CashFlowId}", cashFlow.CashFlowId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("CashFlow description patched successfully for CashFlow ID: {CashFlowId}", cashFlow.CashFlowId);

        logger.LogInformation("PatchCashFlowId operation completed successfully for protocol ID: {ProtocolId}", protocol.ProtocolId);

        return Error.SetSuccess();
    }
}
