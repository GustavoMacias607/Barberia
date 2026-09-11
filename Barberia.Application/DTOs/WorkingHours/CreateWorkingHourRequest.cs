namespace Barberia.Application.DTOs.WorkingHours;

public class CreateWorkingHourRequest
{
    public int BarberId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}