using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;

public record CreateAppointmentResult(
    CreateAppointmentStatus Status,
    Appointment? Appointment);
