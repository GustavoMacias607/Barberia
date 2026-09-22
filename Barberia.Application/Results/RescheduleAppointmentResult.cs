using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;

public record RescheduleAppointmentResult(
    RescheduleAppointmentStatus Status,
    Appointment? Appointment);
