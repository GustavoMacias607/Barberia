using Barberia.Application.DTOs.Appointment;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;
using Barberia.Domain.Enums;
using Barberia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly BarberiaDbContext _dbContext;

    public AppointmentRepository(BarberiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _dbContext.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync();
        return appointment;
    }

    public async Task<IEnumerable<BarberAppointmentCount>>
       GetConfirmedAppointmentCountsAsync(
           IEnumerable<int> barberIds,
           DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _dbContext.Appointments
            .Where(x => barberIds.Contains(x.BarberId))
            .Where(x => x.Status == AppointmentStatus.Confirmed)
            .Where(x => x.StartAt >= dayStart && x.StartAt < dayEnd)
            .GroupBy(x => x.BarberId)
            .Select(x => new BarberAppointmentCount(
                x.Key,
                x.Count()))
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingConfirmedAppointmentAsync(
      int barberId,
      DateTime startAt,
      DateTime endAt)
    {
        return await _dbContext.Appointments
            .Where(x => x.BarberId == barberId)
            .Where(x => x.Status == AppointmentStatus.Confirmed)
            .Where(x =>
                x.StartAt < endAt &&
                x.StartAt.AddMinutes(x.DurationMinutes) > startAt)
            .AnyAsync();
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment)
    {
        _dbContext.Appointments.Update(appointment);
        await _dbContext.SaveChangesAsync();

        return appointment;
    }
}
