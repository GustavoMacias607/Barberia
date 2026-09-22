using Barberia.Application.DTOs.Appointment;
using Barberia.Domain.Entities;

namespace Barberia.Application.Interfaces.Repositories;

public interface IAppointmentRepository

{
    Task<bool> HasOverlappingConfirmedAppointmentAsync(
        int barberId,
        DateTime startAt,
        DateTime endAt,
        int? excludedAppointmentId = null);

    Task<IEnumerable<BarberAppointmentCount>> GetConfirmedAppointmentCountsAsync(
        IEnumerable<int> barberIds,
        DateTime date,
        int? excludedAppointmentId = null);

    Task<Appointment> CreateAsync(Appointment appointment);

    Task<Appointment?> GetByIdAsync(int id);

    Task<Appointment> UpdateAsync(Appointment appointment);

    Task<IEnumerable<Appointment>> GetConfirmedByBarbersAndDateAsync(
        IEnumerable<int> barberIds,
        DateTime date);

    Task<bool> HasOverlappingConfirmedAppointmentForCustomerAsync(
    int customerId,
    DateTime startAt,
    DateTime endAt,
    int? excludedAppointmentId = null);

    Task<IEnumerable<Appointment>> GetByDateAsync(
       DateTime date,
       int? barberId = null);
}
