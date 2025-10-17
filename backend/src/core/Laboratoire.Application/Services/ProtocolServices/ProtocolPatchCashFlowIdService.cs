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
        var description = protocolDto.ToCashFlow()?.Description;
        var ok = await protocolRepository.PatchReportAsync(protocol,description);
        if (!ok)
        {
            logger.LogError("Failed to update CashFlowId for protocol ID {ProtocolId}.", protocol.ProtocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("CashFlowId updated successfully for protocol ID: {ProtocolId}", protocol.ProtocolId);

        return Error.SetSuccess();
    }
}
