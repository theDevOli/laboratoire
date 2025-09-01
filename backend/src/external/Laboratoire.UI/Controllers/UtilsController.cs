using Microsoft.AspNetCore.Mvc;

using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Policy =Policy.All)]
public class UtilsController
(
    IStateGetterService stateGetterService
)
: ControllerBase
{
    [HttpGet("States")]
    public async Task<IActionResult> GetAllStatesAsync()
    {
        var states = await stateGetterService.GetAllStatesAsync();
        if (states is null)
            return StatusCode(200, ApiResponse<IEnumerable<State>>.Success(Array.Empty<State>()));

        return StatusCode(200, ApiResponse<IEnumerable<State>>.Success(states));
    }
}