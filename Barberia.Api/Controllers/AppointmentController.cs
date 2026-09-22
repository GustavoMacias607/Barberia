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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetByDate(
        [FromQuery] DateTime date,
        [FromQuery] int? barberId = null)
    {
        var appointments = await _appointmentService.GetByDateAsync(
            date,
            barberId);

        var response = appointments.Select(appointment =>
            new AppointmentResponse(
                appointment.Id,
                appointment.CustomerId,
                appointment.BarberId,
                appointment.ServiceId,
                appointment.StartAt,
                appointment.DurationMinutes,
                appointment.Status,
                appointment.CreatedAt
            ));

        return Ok(response);
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

        if (result.Status == CreateAppointmentStatus.CustomerHasOverlappingAppointment)
        {
            return Conflict("Customer already has an overlapping appointment");
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

    [HttpPatch("{id}/cancel")]
    public async Task<ActionResult<AppointmentResponse>> Cancel(int id)
    {
        var result = await _appointmentService.CancelAsync(id);

        if (result.Status == CancelAppointmentStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status == CancelAppointmentStatus.CannotCancelCompleted)
        {
            return Conflict("Completed appointments cannot be cancelled");
        }

        if ( result.Status != CancelAppointmentStatus.Success)
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

        return Ok(response);
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<AppointmentResponse>> Complete(int id)
    {
        var result = await _appointmentService.CompleteAsync(id);

        if (result.Status == CompleteAppointmentStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status == CompleteAppointmentStatus.CannotCompleteCancelled)
        {
            return Conflict("Cancelled appointments cannot be completed");
        }

        if (result.Status != CompleteAppointmentStatus.Success)
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

        return Ok(response);
    }

    [HttpPatch("{id}/reschedule")]
    public async Task<ActionResult<AppointmentResponse>> Reschedule(
        int id,
        RescheduleAppointmentRequest request){
        var result = await _appointmentService.RescheduleAsync(id, request);

        if (result.Status == RescheduleAppointmentStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status == RescheduleAppointmentStatus.CannotReschedule)
        {
            return Conflict("Only confirmed appointments can be rescheduled");
        }

        if (result.Status == RescheduleAppointmentStatus.InvalidStartTime)
        {
            return BadRequest("Invalid start time");
        }

        if (result.Status == RescheduleAppointmentStatus.CustomerHasOverlappingAppointment)
        {
            return Conflict("Customer already has an overlapping appointment");
        }

        if (result.Status == RescheduleAppointmentStatus.NoAvailability)
        {
            return Conflict("No availability");
        }

        if (result.Status != RescheduleAppointmentStatus.Success)
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

        return Ok(response);
    }
}
