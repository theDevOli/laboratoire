using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Policy =Policy.Workers)]
public class ClientController
(
    IClientGetterService clientGetterService,
    IClientGetterByIdService clientGetterByIdService,
    IClientGetterByTaxIdService clientGetterByTaxIdService,
    IClientGetterByLikeTaxIdService clientGetterByLikeTaxIdService,
    IClientAdderService clientAdderService,
    IClientUpdatableService clientUpdatableService
)
: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllClientsAsync([FromQuery] string? filter)
    {
        var clients = await clientGetterService.GetAllClientsAsync(filter);
        return Ok(ApiResponse<IEnumerable<Client>>.Success(clients));
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClientByIdAsync([FromRoute] Guid clientId)
    {
        var client = await clientGetterByIdService.GetClientByIdAsync(clientId);
        if (client is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Client>.Success(client));
    }

    [HttpGet("Like/TaxId")]
    public async Task<IActionResult> GetClientsByLikeTaxIdAsync([FromQuery] string filter)
    {
        var clients = await clientGetterByLikeTaxIdService.GetClientsByLikeTaxId(filter);
        return Ok(ApiResponse<IEnumerable<Client>>.Success(clients));
    }

    [HttpGet("TaxId/{clientTaxId}")]
    public async Task<IActionResult> GetClientByTaxIdAsync([FromRoute] string clientTaxId)
    {
        var client = await clientGetterByTaxIdService.GetClientByTaxIdAsync(clientTaxId);
        if (client is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Client>.Success(client));
    }

    [HttpPost]
    public async Task<IActionResult> AddClientAsync([FromBody] ClientDtoAdd clientDto)
    {
        var addError = await clientAdderService.AddClientAsync(clientDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<object>.Success(SuccessMessage.Added));
    }

    [HttpPut("{clientId}")]
    public async Task<IActionResult> UpdateClientAsync([FromRoute] Guid clientId, [FromBody] Client client)
    {
        if (clientId != client.ClientId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await clientUpdatableService.UpdateClientAsync(client);

        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<object>.Success(SuccessMessage.Updated));
    }
}