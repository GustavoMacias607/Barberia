using Barberia.Domain.Entities;

namespace Barberia.Application.Interfaces.Repositories;

public interface IWorkingHourRepository
{
    Task<IEnumerable<WorkingHour>> GetByBarberIdAsync(int barberId);
    Task<WorkingHour?> GetByIdAsync(int id);
    Task<bool> HasConflictAsync(
    int barberId,
    DayOfWeek dayOfWeek,
    TimeOnly startTime,
    TimeOnly endTime);
    Task<WorkingHour> CreateAsync(WorkingHour workingHour);
    Task DeleteAsync(int id);

    Task<IEnumerable<WorkingHour>> GetByBarberAndDayAsync(
    int barberId,
    DayOfWeek dayOfWeek);

    Task<IEnumerable<WorkingHour>> GetByBarbersAndDayAsync(
    IEnumerable<int> barberIds,
    DayOfWeek dayOfWeek);
}