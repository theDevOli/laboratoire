using Microsoft.AspNetCore.Mvc;

using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Roles = "admin,químico")]
public class ChemicalController
(
    IChemicalGetterService chemicalGetterService,
    IChemicalGetterByIdService chemicalGetterByIdService,
    IChemicalUpdatableService chemicalUpdatableService,
    IChemicalAdderService chemicalAdderService
)
: ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllChemicals()
    {
        var chemicals = await chemicalGetterService.GetAllChemicalsAsync();
        return Ok(ApiResponse<IEnumerable<ChemicalDtoGetUpdate>>.Success(chemicals));
    }

    [HttpGet("{chemicalId}")]
    public async Task<IActionResult> GetChemicalByIdAsync([FromRoute] int chemicalId)
    {
        var chemical = await chemicalGetterByIdService.GetChemicalByIdAsync(chemicalId);

        if (chemical is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<ChemicalDtoGetUpdate>.Success(chemical));
    }

    [HttpPut("{chemicalId}")]
    public async Task<IActionResult> UpdateChemicalAsync([FromRoute] int chemicalId, [FromBody] ChemicalDtoGetUpdate chemicalDto)
    {
        if (chemicalId != chemicalDto.ChemicalId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await chemicalUpdatableService.UpdateChemicalAsync(chemicalDto);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [HttpPost]
    public async Task<IActionResult> AddChemicalAsync([FromBody] ChemicalDtoAdd ChemicalDto)
    {
        var addError = await chemicalAdderService.AddChemicalAsync(ChemicalDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }
}