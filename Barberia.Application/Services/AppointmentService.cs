using Barberia.Application.DTOs.Appointment;
using Barberia.Application.Enums;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Application.Interfaces.Transactions;
using Barberia.Application.Results;
using Barberia.Domain.Entities;

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

    public async Task<CreateAppointmentResult> CreateAsync(CreateAppointmentRequest request)
    {

        if (request.StartAt.Minute % 30 != 0 || request.StartAt.Second != 0 || request.StartAt.Millisecond != 0)
        {
            return new CreateAppointmentResult(CreateAppointmentStatus.InvalidStartTime, null);
        }

        if (request.StartAt < DateTime.Now)
        {
            return new CreateAppointmentResult(CreateAppointmentStatus.InvalidStartTime, null);
        }

        return await _transactionManager.ExecuteSerializableAsync(async () =>
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer is null)
            {
                return new CreateAppointmentResult(CreateAppointmentStatus.CustomerNotFound, null);
            }

            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service is null)
            {
                return new CreateAppointmentResult(CreateAppointmentStatus.ServiceNotFound, null);
            }
            if (!service.IsActive)
            {
                return new CreateAppointmentResult(CreateAppointmentStatus.ServiceInactive, null);
            }

            var endAt = request.StartAt.AddMinutes(service.DurationMinutes);

            var availableBarbers = new List<Barber>();

            var activeBarbers = await _barberRepository.GetActiveAsync();

            foreach (var barber in activeBarbers)
            {
                var schedules = await _workingHourRepository.GetByBarberAndDayAsync(barber.Id, request.StartAt.DayOfWeek);
                foreach (var schedule in schedules)
                {
                    var scheduleStartAt =
                        request.StartAt.Date.Add(schedule.StartTime.ToTimeSpan());

                    var scheduleEndAt =
                        request.StartAt.Date.Add(schedule.EndTime.ToTimeSpan());

                    if (request.StartAt >= scheduleStartAt &&
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
                var hasOverlapping = await _appointmentRepository.HasOverlappingConfirmedAppointmentAsync(barber.Id, request.StartAt, endAt);
                if (!hasOverlapping)
                {
                    freeBarbers.Add(barber);
                }
            }

            if (freeBarbers.Count == 0)
            {
                return new CreateAppointmentResult(CreateAppointmentStatus.NoAvailability, null);
            }

            var barberIds = freeBarbers.Select(x => x.Id);

            var confirmedAppointmentCounts = await _appointmentRepository.GetConfirmedAppointmentCountsAsync(barberIds, request.StartAt.Date);

            var barberWithLeastAppointments = freeBarbers
                .Select(barber => new
                {
                    Barber = barber,
                    AppointmentCount = confirmedAppointmentCounts
                        .FirstOrDefault(x => x.BarberId == barber.Id)?.ConfirmedCount ?? 0
                })
                .OrderBy(x => x.AppointmentCount)
                .ThenBy(x => x.Barber.Name, StringComparer.OrdinalIgnoreCase)
                .First();

            Appointment appointment = new(
                customer.Id,
                barberWithLeastAppointments.Barber.Id,
                service.Id,
                request.StartAt,
                service.DurationMinutes);


            var created = await _appointmentRepository.CreateAsync(appointment);

            return new CreateAppointmentResult(CreateAppointmentStatus.Success, created);
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
}
