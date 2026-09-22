using Barberia.Application.DTOs.Availability;
using Barberia.Application.Enums;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Application.Results;

namespace Barberia.Application.Services;

public class AvailabilityService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IBarberRepository _barberRepository;
    private readonly IWorkingHourRepository _workingHourRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public AvailabilityService(
        IServiceRepository serviceRepository,
        IBarberRepository barberRepository,
        IWorkingHourRepository workingHourRepository,
        IAppointmentRepository appointmentRepository)
    {
        _serviceRepository = serviceRepository;
        _barberRepository = barberRepository;
        _workingHourRepository = workingHourRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AvailabilityResult> GetAsync(AvailabilityRequest request)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);

        if (service is null)
        {
            return new AvailabilityResult(
                AvailabilityStatus.ServiceNotFound,
                null);
        }

        if (!service.IsActive)
        {
            return new AvailabilityResult(
                AvailabilityStatus.ServiceInactive,
                null);
        }

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);

        if (request.Date < today)
        {
            AvailabilityResponse response = new()
            {
                ServiceId = service.Id,
                Date = request.Date,
                AvailableSlots = []
            };

            return new AvailabilityResult(
                AvailabilityStatus.Success,
                response);
        }

        var activeBarbers = (await _barberRepository.GetActiveAsync())
            .ToList();

        var barberIds = activeBarbers
            .Select(x => x.Id)
            .ToList();

        if (barberIds.Count == 0)
        {
            AvailabilityResponse response = new()
            {
                ServiceId = service.Id,
                Date = request.Date,
                AvailableSlots = []
            };

            return new AvailabilityResult(
                AvailabilityStatus.Success,
                response);
        }

        var schedules = await _workingHourRepository
            .GetByBarbersAndDayAsync(
                barberIds,
                request.Date.DayOfWeek);

        var appointments = await _appointmentRepository
            .GetConfirmedByBarbersAndDateAsync(
                barberIds,
                request.Date.ToDateTime(TimeOnly.MinValue));

        var schedulesByBarber = schedules
            .ToLookup(x => x.BarberId);

        var appointmentsByBarber = appointments
            .ToLookup(x => x.BarberId);

        var availableSlots = new HashSet<TimeOnly>();

        foreach (var barber in activeBarbers)
        {
            foreach (var schedule in schedulesByBarber[barber.Id])
            {
                var scheduleStartAt =
                    request.Date.ToDateTime(schedule.StartTime);

                var scheduleEndAt =
                    request.Date.ToDateTime(schedule.EndTime);

                var slotStart =
                    request.Date.ToDateTime(TimeOnly.MinValue);

                while (slotStart < scheduleStartAt)
                {
                    slotStart = slotStart.AddMinutes(30);
                }

                while (
                    slotStart.AddMinutes(service.DurationMinutes)
                    <= scheduleEndAt)
                {
                    var slotEnd =
                        slotStart.AddMinutes(service.DurationMinutes);

                    if (slotStart >= now)
                    {
                        var hasOverlapping =
                            appointmentsByBarber[barber.Id]
                                .Any(x =>
                                    x.StartAt < slotEnd &&
                                    x.StartAt
                                        .AddMinutes(x.DurationMinutes)
                                        > slotStart);

                        if (!hasOverlapping)
                        {
                            availableSlots.Add(
                                TimeOnly.FromDateTime(slotStart));
                        }
                    }

                    slotStart = slotStart.AddMinutes(30);
                }
            }
        }

        AvailabilityResponse availabilityResponse = new()
        {
            ServiceId = service.Id,
            Date = request.Date,
            AvailableSlots = availableSlots
                .OrderBy(x => x)
                .ToList()
        };

        return new AvailabilityResult(
            AvailabilityStatus.Success,
            availabilityResponse);
    }
}
