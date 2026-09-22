namespace Barberia.Application.Enums;

public enum CreateAppointmentStatus
{
    Success,
    CustomerNotFound,
    ServiceNotFound,
    ServiceInactive,
    InvalidStartTime,
    CustomerHasOverlappingAppointment,
    NoAvailability
}
