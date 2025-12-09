using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Policy = Policy.Workers)]
public class OfficeController
(
    IOfficeAdderService officeAdderService,
    IOfficeGetterByIdService officeGetterByIdService,
    IOfficeGetterService officeGetterService,
    IOfficeUpdatableService officeUpdatableService
)
: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllOfficesAsync()
    {
        var offices = await officeGetterService.GetAllOfficesAsync();

        return Ok(ApiResponse<IEnumerable<Office>>.Success(offices));
    }

    [HttpGet("{officeId}")]
    public async Task<IActionResult> GetOfficeByOfficeIdAsync([FromRoute] Guid officeId)
    {
        var office = await officeGetterByIdService.GetByOfficeIdAsync(officeId);

        if (office is null)
            return StatusCode(404, ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Office>.Success(office));
    }

    [HttpPost]
    public async Task<IActionResult> AddOfficeAsync([FromBody] OfficeDtoUpsert dto)
    {
        var office = dto.ToOffice();

        var error = await officeAdderService.AddOfficeAsync(office);

        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error.Message!, error!.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{officeId}")]
    public async Task<IActionResult> UpdateOfficeAsync([FromBody] OfficeDtoUpsert dto, [FromRoute] Guid officeId)
    {
        var office = dto.ToOffice(officeId);

        var error = await officeUpdatableService.UpdateOfficeAsync(office);

        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error.Message!, error.StatusCode!));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}

