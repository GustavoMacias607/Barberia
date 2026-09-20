namespace Barberia.Application.Enums;

public enum CreateAppointmentStatus
{
    CustomerNotFound,
    ServiceNotFound,
    ServiceInactive,
    InvalidStartTime,
    NoAvailability,
    Success
}
