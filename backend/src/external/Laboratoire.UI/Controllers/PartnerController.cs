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
[Authorize(Policy = Policy.Workers)]
public class PartnerController
(
    IPartnerGetterService partnerGetterService,
    IPartnerGetterByIdService partnerGetterByIdService,
    IPartnerAdderService partnerAdderService,
    IPartnerUpdatableService partnerUpdatableService
)
: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPartnersAsync()
    {
        var partners = await partnerGetterService.GetAllPartnersAsync();
        return Ok(ApiResponse<IEnumerable<Partner>>.Success(partners));
    }

    [HttpGet("{partnerId}")]
    public async Task<IActionResult> GetPartnerByIdAsync([FromRoute] Guid partnerId)
    {
        var partner = await partnerGetterByIdService.GetPartnerByIdAsync(partnerId);
        if (partner is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Partner>.Success(partner));
    }

    [HttpPut("{partnerId}")]
    public async Task<IActionResult> UpdatePartnerAsync([FromRoute] Guid partnerId, [FromBody] Partner partner)
    {
        if (partnerId != partner.PartnerId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await partnerUpdatableService.UpdatePartnerAsync(partner);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [HttpPost]
    public async Task<IActionResult> AddPartnerAsync([FromBody] PartnerDtoAdd partnerDto)
    {
        var addError = await partnerAdderService.AddPartnerAsync(partnerDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }
}