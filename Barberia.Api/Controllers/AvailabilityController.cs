using Barberia.Application.DTOs.Availability;
using Barberia.Application.Enums;
using Barberia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly AvailabilityService _availabilityService;

    public AvailabilityController(AvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet]
    public async Task<ActionResult<AvailabilityResponse>> Get(
        [FromQuery] AvailabilityRequest request)
    {
        var result = await _availabilityService.GetAsync(request);

        if (result.Status == AvailabilityStatus.ServiceNotFound)
        {
            return NotFound("Service not found");
        }

        if (result.Status == AvailabilityStatus.ServiceInactive)
        {
            return BadRequest("Service inactive");
        }

        if (result.Status != AvailabilityStatus.Success)
        {
            return StatusCode(500);
        }

        return Ok(result.Availability);
    }
}
