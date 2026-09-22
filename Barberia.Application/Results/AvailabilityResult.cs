using Barberia.Application.DTOs.Availability;
using Barberia.Application.Enums;

namespace Barberia.Application.Results;

public record AvailabilityResult(
    AvailabilityStatus Status,
    AvailabilityResponse? Availability);