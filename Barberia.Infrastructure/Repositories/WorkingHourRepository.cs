using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;
using Barberia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Infrastructure.Repositories;

public class WorkingHourRepository : IWorkingHourRepository
{
    private readonly BarberiaDbContext _dbContext;

    public WorkingHourRepository(BarberiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkingHour?> GetByIdAsync(int id)
    {
        return await _dbContext.WorkingHours
            .Include(x => x.Barber)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<WorkingHour>> GetByBarberIdAsync(int barberId)
    {
        return await _dbContext.WorkingHours
            .Include(x => x.Barber)
            .Where(x => x.BarberId == barberId)
            .ToListAsync();
    }
    public async Task<WorkingHour> CreateAsync(WorkingHour workingHour)
    {
        _dbContext.WorkingHours.Add(workingHour);
        await _dbContext.SaveChangesAsync();
        return workingHour;
    }

    public async Task DeleteAsync(int id)
    {
        await _dbContext.WorkingHours
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }
    public async Task<bool> HasConflictAsync(int barberId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        return await _dbContext.WorkingHours.AnyAsync(x =>
            x.BarberId == barberId &&
            x.DayOfWeek == dayOfWeek &&
            startTime < x.EndTime &&
            endTime > x.StartTime
        );
    }
}