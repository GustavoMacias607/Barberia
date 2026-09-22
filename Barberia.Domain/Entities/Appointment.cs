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

    public bool Cancel()
    {
        if(Status == AppointmentStatus.Completed)
        {
            return false;
        }
        Status = AppointmentStatus.Cancelled;
        return true;
    }

    public bool Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
        {
            return false;
        }
        Status = AppointmentStatus.Completed;
        return true;
    }

    public bool Reschedule(int barberId, DateTime startAt)
    {
        if (Status != AppointmentStatus.Confirmed)
        {
            return false;
        }

        BarberId = barberId;
        StartAt = startAt;

        return true;
    }

    public bool CanBeRescheduled => Status == AppointmentStatus.Confirmed;
}
