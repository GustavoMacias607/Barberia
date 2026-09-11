namespace Barberia.Application.DTOs.WorkingHours;

public class WorkingHourResponse{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public string BarberName { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public WorkingHourResponse(int id, int barberId,string barberName, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        Id = id;
        BarberId = barberId;
        BarberName = barberName;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
}