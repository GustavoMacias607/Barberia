using Barberia.Domain.Enums;

namespace Barberia.Application.DTOs.Appointment;

public class AppointmentResponse
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public int BarberId { get; private set; }
    public int ServiceId { get; private set; }
    public DateTime StartAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AppointmentResponse(int id, int customerId, int barberId, int serviceId, DateTime startAt, int durationMinutes, AppointmentStatus status, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        BarberId = barberId;
        ServiceId = serviceId;
        StartAt = startAt;
        DurationMinutes = durationMinutes;
        Status = status;
        CreatedAt = createdAt;
    }
}
