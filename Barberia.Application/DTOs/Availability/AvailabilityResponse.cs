namespace Barberia.Application.DTOs.Availability;

public class AvailabilityResponse
{
    public int ServiceId { get; set; }
    public DateOnly Date { get; set; }
    public IEnumerable<TimeOnly>? AvailableSlots { get; set; }
}
