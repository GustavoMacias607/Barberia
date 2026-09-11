using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;

public record GetWorkingHoursByBarberResult(
    GetWorkingHoursByBarberStatus Status,
    IEnumerable<WorkingHour>? WorkingHours
);