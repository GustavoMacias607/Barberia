namespace Barberia.Domain.Entities;

public class WorkingHour
{
    public int Id { get; private set; }
    public int BarberId { get; private set; }
    public Barber Barber { get; private set; } = null!;
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    public WorkingHour(int barberId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        BarberId = barberId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
}

