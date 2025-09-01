using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Policy ="workers")]
public class PropertyController
(
    IPropertyGetterService propertyGetterService,
    IPropertyGetterByPropertyIdService propertyGetterByPropertyIdService,
    IPropertyGetterByClientIdService propertyGetterByClientIdService,
    IPropertyGetterToDisplayService propertyGetterToDisplayService,
    IPropertyAdderService propertyAdderService,
    IPropertyUpdatableService propertyUpdatableService
)
: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPropertiesAsync()
    {
        var properties = await propertyGetterService.GetAllPropertiesAsync();
        return Ok(ApiResponse<IEnumerable<Property>>.Success(properties));
    }

    [HttpGet("Display")]
    public async Task<IActionResult> GetAllPropertiesToDisplayAsync()
    {
        var properties = await propertyGetterToDisplayService.GetAllPropertiesDisplayAsync();
        return Ok(ApiResponse<IEnumerable<PropertyDtoDisplay>>.Success(properties));
    }

    [HttpGet("{propertyId}")]
    public async Task<IActionResult> GetPropertyByIdAsync([FromRoute] int propertyId)
    {
        var property = await propertyGetterByPropertyIdService.GetPropertyByPropertyIdAsync(propertyId);
        if (property is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Property>.Success(property));
    }

    [HttpGet("Client/{clientId}")]
    public async Task<IActionResult> GetPropertyByClientIdAsync([FromRoute] Guid? clientId)
    {
        var property = await propertyGetterByClientIdService.GetAllPropertiesByClientIdAsync(clientId);
        if (property is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<IEnumerable<Property>>.Success(property));
    }

    [HttpPost]
    public async Task<IActionResult> AddPropertyAsync([FromBody] PropertyDtoAdd propertyDto)
    {
        var addError = await propertyAdderService.AddPropertyAsync(propertyDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{propertyId}")]
    public async Task<IActionResult> UpdatePropertyAsync([FromRoute] int propertyId, [FromBody] Property property)
    {
        if (propertyId != property.PropertyId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await propertyUpdatableService.UpdatePropertyAsync(property);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}