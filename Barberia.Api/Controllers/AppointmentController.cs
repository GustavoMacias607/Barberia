using Barberia.Application.DTOs.Appointment;
using Barberia.Application.Enums;
using Barberia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AppointmentController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(int id)
    {
        var result = await _appointmentService.GetByIdAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        AppointmentResponse response = new(
           result.Id,
           result.CustomerId,
           result.BarberId,
           result.ServiceId,
           result.StartAt,
           result.DurationMinutes,
           result.Status,
           result.CreatedAt
           );

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create(CreateAppointmentRequest createAppointmentRequest)
    {
        var result = await _appointmentService.CreateAsync(createAppointmentRequest);

        if(result.Status == CreateAppointmentStatus.CustomerNotFound)
        {
            return NotFound("Customer not found");
        }

        if (result.Status == CreateAppointmentStatus.ServiceNotFound)
        {
            return NotFound("Service not found");
        }

        if (result.Status == CreateAppointmentStatus.ServiceInactive)
        {
            return BadRequest("Service Inactive");
        }

        if (result.Status == CreateAppointmentStatus.InvalidStartTime)
        {
            return BadRequest("Invalid start time");
        }

        if (result.Status == CreateAppointmentStatus.NoAvailability)
        {
            return Conflict("No Availability");
        }

        if (result.Status != CreateAppointmentStatus.Success)
        {
            return StatusCode(500);
        }

        AppointmentResponse response = new(
            result.Appointment!.Id,
            result.Appointment.CustomerId,
            result.Appointment.BarberId,
            result.Appointment.ServiceId,
            result.Appointment.StartAt,
            result.Appointment.DurationMinutes,
            result.Appointment.Status,
            result.Appointment.CreatedAt
            );

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }
}
