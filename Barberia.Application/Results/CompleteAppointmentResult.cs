using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;
public record CompleteAppointmentResult(
    CompleteAppointmentStatus Status,
    Appointment? Appointment);
