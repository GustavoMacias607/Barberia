using Barberia.Domain.Enums;

namespace Barberia.Domain.Entities;

public class Appointment
{
    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public int BarberId { get; private set; }
    public int ServiceId { get; private set; }
    public DateTime StartAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Appointment(int customerId, int barberId, int serviceId, DateTime startAt, int durationMinutes)
    {
        CustomerId = customerId;
        BarberId = barberId;
        ServiceId = serviceId;
        StartAt = startAt;
        DurationMinutes = durationMinutes;
        Status = AppointmentStatus.Confirmed;
        CreatedAt = DateTime.Now;
    }
}

