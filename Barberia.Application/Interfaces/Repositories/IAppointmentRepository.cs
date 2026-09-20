using Barberia.Application.DTOs.Appointment;
using Barberia.Domain.Entities;

namespace Barberia.Application.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task<bool> HasOverlappingConfirmedAppointmentAsync(
    int barberId,
    DateTime startAt,
    DateTime endAt);

    Task<IEnumerable<BarberAppointmentCount>> GetConfirmedAppointmentCountsAsync(
        IEnumerable<int> barberIds,
        DateTime date);

    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync (int id);
}
