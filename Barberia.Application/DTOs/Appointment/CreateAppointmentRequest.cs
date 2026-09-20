namespace Barberia.Application.DTOs.Appointment;

public class CreateAppointmentRequest
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public DateTime StartAt { get; set; }
}
