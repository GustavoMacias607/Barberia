namespace Barberia.Application.DTOs.Appointment;

public class BarberAppointmentCount
{
    public int BarberId { get; set; }
    public int ConfirmedCount { get; set; }

    public BarberAppointmentCount(int barberId, int confirmedCount)
    {
        BarberId = barberId;
        ConfirmedCount = confirmedCount;
    }
}
