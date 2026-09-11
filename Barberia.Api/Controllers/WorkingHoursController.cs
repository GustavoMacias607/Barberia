using Barberia.Application.DTOs.WorkingHours;
using Barberia.Application.Enums;
using Barberia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class WorkingHoursController : ControllerBase
{
    private readonly WorkingHourService _workingHourService;
    public WorkingHoursController(WorkingHourService workingHourService)
    {
        _workingHourService = workingHourService;
    }

    [HttpGet("barber/{barberId}")]
    public async Task<ActionResult<IEnumerable<WorkingHourResponse>>> GetByBarberId(int barberId)
    {
        var response = await _workingHourService.GetByBarberIdAsync(barberId);
        if (response.Status == GetWorkingHoursByBarberStatus.BarberNotFound)
        {
            return NotFound();
        }

        return Ok(response.WorkingHours!.Select(x => new WorkingHourResponse(
            x.Id,
            x.BarberId,
            x.Barber.Name,
            x.DayOfWeek,
            x.StartTime,
            x.EndTime
            )));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkingHourResponse>> GetById(int id)
    {
        var response = await _workingHourService.GetByIdAsync(id);

        if(response is null)
        {
            return NotFound();
        }
        WorkingHourResponse workingHour = new(
            response.Id,
            response.BarberId,
            response.Barber.Name,
            response.DayOfWeek,
            response.StartTime,
            response.EndTime
            );
        return Ok(workingHour);
    }

    [HttpPost]
    public async Task<ActionResult<WorkingHourResponse>> Create(CreateWorkingHourRequest request)
    {
        var response = await _workingHourService.CreateAsync(request);

        if (response.Status == CreateWorkingHourStatus.BarberNotFound)
        {
            return NotFound();
        }

        if (response.Status == CreateWorkingHourStatus.InvalidTime)
        {
            return BadRequest();
        }

        if (response.Status == CreateWorkingHourStatus.Conflict)
        {
            return Conflict();
        }

        WorkingHourResponse workingHour = new(
            response.WorkingHour!.Id,
            response.WorkingHour.BarberId,
            response.WorkingHour.Barber.Name,
            response.WorkingHour.DayOfWeek,
            response.WorkingHour.StartTime,
            response.WorkingHour.EndTime
            );

        return CreatedAtAction(nameof(GetById), new { id = response.WorkingHour.Id }, workingHour);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var response = await _workingHourService.DeleteAsync(id);
        if(!response)
        {
            return NotFound();
        }

        return NoContent();
    }
}