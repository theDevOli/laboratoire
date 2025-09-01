using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laboratoire.UI.Controllers
{
    [ApiController]
    [Route("v1/api/[controller]")]
    [Authorize(Policy = Policy.Workers)]
    public class FertilizerController
    (IFertilizerGetterService fertilizerGetterService)
    : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllFertilizersAsync()
        {
            var fertilizers = await fertilizerGetterService.GetAllFertilizersAsync();
            return Ok(ApiResponse<IEnumerable<FertilizerDtoGet>>.Success(fertilizers));

        }
    }
}