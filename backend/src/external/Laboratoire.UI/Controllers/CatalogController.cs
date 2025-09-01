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
public class CatalogController
(
    ICatalogGetterService catalogGetterService,
    ICatalogGetterByIdService catalogGetterByIdService,
    ICatalogAdderService catalogAdderService,
    ICatalogUpdatableService catalogUpdatableService
)
: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCatalogsAsync()
    {
        var catalogs = await catalogGetterService.GetAllCatalogsAsync();
        return Ok(ApiResponse<IEnumerable<Catalog>>.Success(catalogs));
    }

    [HttpGet("{catalogId}")]
    public async Task<IActionResult> GetCatalogByIdAsync([FromRoute] int catalogId)
    {
        var catalog = await catalogGetterByIdService.GetCatalogByIdAsync(catalogId);
        if (catalog is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Catalog>.Success(catalog));
    }

    [HttpPost]
    public async Task<IActionResult> AddCatalogAsync([FromBody] CatalogDtoAdd dto)
    {
        var addError = await catalogAdderService.AddCatalogAsync(dto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{catalogId}")]
    public async Task<IActionResult> UpdateCatalogsAsync([FromRoute] int catalogId, [FromBody] Catalog catalog)
    {
        if (catalogId != catalog.CatalogId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await catalogUpdatableService.UpdateCatalogAsync(catalog);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}