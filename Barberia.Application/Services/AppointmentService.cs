using Barberia.Application.DTOs.Appointment;
using Barberia.Application.Enums;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Application.Interfaces.Transactions;
using Barberia.Application.Results;
using Barberia.Domain.Entities;
using Barberia.Domain.Enums;

namespace Barberia.Application.Services;

public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBarberRepository _barberRepository;
    private readonly IWorkingHourRepository _workingHourRepository;
    private readonly ITransactionManager _transactionManager;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository,
        IServiceRepository serviceRepository,
        IBarberRepository barberRepository,
        IWorkingHourRepository workingHourRepository,
        ITransactionManager transactionManager)
    {
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
        _serviceRepository = serviceRepository;
        _barberRepository = barberRepository;
        _workingHourRepository = workingHourRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _appointmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Appointment>> GetByDateAsync(
        DateTime date,
        int? barberId = null,
        AppointmentStatus? status = null)
    {
        return await _appointmentRepository.GetByDateAsync(
            date,
            barberId,
            status);
    }

    public async Task<CreateAppointmentResult> CreateAsync(
        CreateAppointmentRequest request)
    {
        if (request.StartAt.Minute % 30 != 0 ||
            request.StartAt.Second != 0 ||
            request.StartAt.Millisecond != 0)
        {
            return new CreateAppointmentResult(
                CreateAppointmentStatus.InvalidStartTime,
                null);
        }

        if (request.StartAt < DateTime.Now)
        {
            return new CreateAppointmentResult(
                CreateAppointmentStatus.InvalidStartTime,
                null);
        }

        return await _transactionManager.ExecuteSerializableAsync(async () =>
        {
            var customer = await _customerRepository
                .GetByIdAsync(request.CustomerId);

            if (customer is null)
            {
                return new CreateAppointmentResult(
                    CreateAppointmentStatus.CustomerNotFound,
                    null);
            }

            var service = await _serviceRepository
                .GetByIdAsync(request.ServiceId);

            if (service is null)
            {
                return new CreateAppointmentResult(
                    CreateAppointmentStatus.ServiceNotFound,
                    null);
            }

            if (!service.IsActive)
            {
                return new CreateAppointmentResult(
                    CreateAppointmentStatus.ServiceInactive,
                    null);
            }

            var endAt = request.StartAt.AddMinutes(service.DurationMinutes);

            var customerHasOverlappingAppointment =
                await _appointmentRepository
                    .HasOverlappingConfirmedAppointmentForCustomerAsync(
                        customer.Id,
                        request.StartAt,
                        endAt);

            if (customerHasOverlappingAppointment)
            {
                return new CreateAppointmentResult(
                    CreateAppointmentStatus.CustomerHasOverlappingAppointment,
                    null);
            }

            var selectedBarber = await FindAvailableBarberAsync(
                request.StartAt,
                service.DurationMinutes);

            if (selectedBarber is null)
            {
                return new CreateAppointmentResult(
                    CreateAppointmentStatus.NoAvailability,
                    null);
            }

            Appointment appointment = new(
                customer.Id,
                selectedBarber.Id,
                service.Id,
                request.StartAt,
                service.DurationMinutes);

            var created = await _appointmentRepository
                .CreateAsync(appointment);

            return new CreateAppointmentResult(
                CreateAppointmentStatus.Success,
                created);
        });
    }

    public async Task<CancelAppointmentResult> CancelAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null)
        {
            return new CancelAppointmentResult(CancelAppointmentStatus.NotFound, null);
        }

       var cancelled = appointment.Cancel();

        if (!cancelled)
        {
            return new CancelAppointmentResult(
                CancelAppointmentStatus.CannotCancelCompleted,
                appointment);
        }
        var updated = await _appointmentRepository.UpdateAsync(appointment);

        return new CancelAppointmentResult(CancelAppointmentStatus.Success, updated);
    }

    public async Task<CompleteAppointmentResult> CompleteAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null)
        {
            return new CompleteAppointmentResult(CompleteAppointmentStatus.NotFound, null);
        }

        var completed = appointment.Complete();

        if (!completed)
        {
            return new CompleteAppointmentResult(
                CompleteAppointmentStatus.CannotCompleteCancelled,
                appointment);
        }

        var updated = await _appointmentRepository.UpdateAsync(appointment);

        return new CompleteAppointmentResult(CompleteAppointmentStatus.Success, updated);
    }

    private async Task<Barber?> FindAvailableBarberAsync(
        DateTime startAt,
        int durationMinutes,
        int? excludedAppointmentId = null)
    {
        var endAt = startAt.AddMinutes(durationMinutes);

        var availableBarbers = new List<Barber>();

        var activeBarbers = await _barberRepository.GetActiveAsync();

        foreach (var barber in activeBarbers)
        {
            var schedules = await _workingHourRepository
                .GetByBarberAndDayAsync(
                    barber.Id,
                    startAt.DayOfWeek);

            foreach (var schedule in schedules)
            {
                var scheduleStartAt =
                    startAt.Date.Add(schedule.StartTime.ToTimeSpan());

                var scheduleEndAt =
                    startAt.Date.Add(schedule.EndTime.ToTimeSpan());

                if (startAt >= scheduleStartAt &&
                    endAt <= scheduleEndAt)
                {
                    availableBarbers.Add(barber);
                    break;
                }
            }
        }

        var freeBarbers = new List<Barber>();

        foreach (var barber in availableBarbers)
        {
            var hasOverlapping =
                await _appointmentRepository
                    .HasOverlappingConfirmedAppointmentAsync(
                        barber.Id,
                        startAt,
                        endAt,
                        excludedAppointmentId);

            if (!hasOverlapping)
            {
                freeBarbers.Add(barber);
            }
        }

        if (freeBarbers.Count == 0)
        {
            return null;
        }

        var barberIds = freeBarbers
            .Select(x => x.Id);

        var confirmedAppointmentCounts =
            await _appointmentRepository
                .GetConfirmedAppointmentCountsAsync(
                    barberIds,
                    startAt.Date,
                    excludedAppointmentId);

        return freeBarbers
            .Select(barber => new
            {
                Barber = barber,
                AppointmentCount = confirmedAppointmentCounts
                    .FirstOrDefault(x => x.BarberId == barber.Id)?
                    .ConfirmedCount ?? 0
            })
            .OrderBy(x => x.AppointmentCount)
            .ThenBy(
                x => x.Barber.Name,
                StringComparer.OrdinalIgnoreCase)
            .First()
            .Barber;
    }

    public async Task<RescheduleAppointmentResult> RescheduleAsync(
        int id,
        RescheduleAppointmentRequest request)
    {
        if (request.StartAt.Minute % 30 != 0 ||
            request.StartAt.Second != 0 ||
            request.StartAt.Millisecond != 0)
        {
            return new RescheduleAppointmentResult(
                RescheduleAppointmentStatus.InvalidStartTime,
                null);
        }

        if (request.StartAt < DateTime.Now)
        {
            return new RescheduleAppointmentResult(
                RescheduleAppointmentStatus.InvalidStartTime,
                null);
        }

        return await _transactionManager.ExecuteSerializableAsync(async () =>
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment is null)
            {
                return new RescheduleAppointmentResult(
                    RescheduleAppointmentStatus.NotFound,
                    null);
            }

            if (!appointment.CanBeRescheduled)
            {
                return new RescheduleAppointmentResult(
                    RescheduleAppointmentStatus.CannotReschedule,
                    appointment);
            }

            var endAt = request.StartAt.AddMinutes(appointment.DurationMinutes);

            var customerHasOverlappingAppointment =
                await _appointmentRepository
                    .HasOverlappingConfirmedAppointmentForCustomerAsync(
                        appointment.CustomerId,
                        request.StartAt,
                        endAt,
                        appointment.Id);

            if (customerHasOverlappingAppointment)
            {
                return new RescheduleAppointmentResult(
                    RescheduleAppointmentStatus.CustomerHasOverlappingAppointment,
                    appointment);
            }

            var selectedBarber = await FindAvailableBarberAsync(
                request.StartAt,
                appointment.DurationMinutes,
                appointment.Id);

            if (selectedBarber is null)
            {
                return new RescheduleAppointmentResult(
                    RescheduleAppointmentStatus.NoAvailability,
                    appointment);
            }

            var rescheduled = appointment.Reschedule(
                selectedBarber.Id,
                request.StartAt);

            if (!rescheduled)
            {
                return new RescheduleAppointmentResult(
                    RescheduleAppointmentStatus.CannotReschedule,
                    appointment);
            }

            var updated = await _appointmentRepository.UpdateAsync(appointment);

            return new RescheduleAppointmentResult(
                RescheduleAppointmentStatus.Success,
                updated);
        });
    }
}
