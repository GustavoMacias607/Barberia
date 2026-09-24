using Barberia.Domain.Enums;

namespace Barberia.Application.DTOs.Appointments;

public record AppointmentAgendaItem(
    int Id,
    int CustomerId,
    string CustomerName,
    int BarberId,
    string BarberName,
    int ServiceId,
    string ServiceName,
    DateTime StartAt,
    int DurationMinutes,
    AppointmentStatus Status,
    DateTime CreatedAt
);
