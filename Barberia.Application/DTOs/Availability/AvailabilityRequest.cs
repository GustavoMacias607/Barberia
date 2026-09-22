namespace Barberia.Application.DTOs.Availability;

public class AvailabilityRequest
{
    public int ServiceId { get; set; }
    public DateOnly Date { get; set; }
}
