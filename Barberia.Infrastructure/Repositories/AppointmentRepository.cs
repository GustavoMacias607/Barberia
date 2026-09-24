using Barberia.Application.DTOs.Appointment;
using Barberia.Application.DTOs.Appointments;
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

    public async Task<IEnumerable<AppointmentAgendaItem>> GetByCustomerIdAsync(
    int customerId)
    {
        var query =
            from appointment in _dbContext.Appointments.AsNoTracking()
            join customer in _dbContext.Customers.AsNoTracking()
                on appointment.CustomerId equals customer.Id
            join barber in _dbContext.Barbers.AsNoTracking()
                on appointment.BarberId equals barber.Id
            join service in _dbContext.Services.AsNoTracking()
                on appointment.ServiceId equals service.Id
            where appointment.CustomerId == customerId
            orderby appointment.StartAt descending
            select new AppointmentAgendaItem(
                appointment.Id,
                appointment.CustomerId,
                customer.Name,
                appointment.BarberId,
                barber.Name,
                appointment.ServiceId,
                service.Name,
                appointment.StartAt,
                appointment.DurationMinutes,
                appointment.Status,
                appointment.CreatedAt
            );

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<BarberAppointmentCount>> GetConfirmedAppointmentCountsAsync(
     IEnumerable<int> barberIds,
     DateTime date,
     int? excludedAppointmentId = null)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _dbContext.Appointments
            .Where(x => barberIds.Contains(x.BarberId))
            .Where(x => x.Status == AppointmentStatus.Confirmed)
            .Where(x =>
                excludedAppointmentId == null ||
                x.Id != excludedAppointmentId.Value)
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
        DateTime endAt,
        int? excludedAppointmentId = null)
    {
        return await _dbContext.Appointments
            .Where(x => x.BarberId == barberId)
            .Where(x => x.Status == AppointmentStatus.Confirmed)
            .Where(x =>
                excludedAppointmentId == null ||
                x.Id != excludedAppointmentId.Value)
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

    public async Task<IEnumerable<Appointment>> GetConfirmedByBarbersAndDateAsync(
    IEnumerable<int> barberIds,
    DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _dbContext.Appointments
            .AsNoTracking()
            .Where(x => barberIds.Contains(x.BarberId))
            .Where(x => x.Status == AppointmentStatus.Confirmed)
            .Where(x =>
                x.StartAt < dayEnd &&
                x.StartAt.AddMinutes(x.DurationMinutes) > dayStart)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingConfirmedAppointmentForCustomerAsync(
    int customerId,
    DateTime startAt,
    DateTime endAt,
    int? excludedAppointmentId = null)
    {
        return await _dbContext.Appointments
            .Where(x => x.CustomerId == customerId)
            .Where(x => x.Status == AppointmentStatus.Confirmed)
            .Where(x =>
                excludedAppointmentId == null ||
                x.Id != excludedAppointmentId.Value)
            .Where(x =>
                x.StartAt < endAt &&
                x.StartAt.AddMinutes(x.DurationMinutes) > startAt)
            .AnyAsync();
    }

    public async Task<IEnumerable<AppointmentAgendaItem>> GetAgendaByDateAsync(
    DateTime date,
    int? barberId = null,
    AppointmentStatus? status = null)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var query =
            from appointment in _dbContext.Appointments.AsNoTracking()
            join customer in _dbContext.Customers.AsNoTracking()
                on appointment.CustomerId equals customer.Id
            join barber in _dbContext.Barbers.AsNoTracking()
                on appointment.BarberId equals barber.Id
            join service in _dbContext.Services.AsNoTracking()
                on appointment.ServiceId equals service.Id
            where appointment.StartAt >= dayStart
                  && appointment.StartAt < dayEnd
            select new
            {
                Appointment = appointment,
                CustomerName = customer.Name,
                BarberName = barber.Name,
                ServiceName = service.Name
            };

        if (barberId.HasValue)
        {
            query = query.Where(x =>
                x.Appointment.BarberId == barberId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.Appointment.Status == status.Value);
        }

        return await query
            .OrderBy(x => x.Appointment.StartAt)
            .Select(x => new AppointmentAgendaItem(
                x.Appointment.Id,
                x.Appointment.CustomerId,
                x.CustomerName,
                x.Appointment.BarberId,
                x.BarberName,
                x.Appointment.ServiceId,
                x.ServiceName,
                x.Appointment.StartAt,
                x.Appointment.DurationMinutes,
                x.Appointment.Status,
                x.Appointment.CreatedAt
            ))
            .ToListAsync();
    }
}
