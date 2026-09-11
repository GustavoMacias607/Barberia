using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;

public record CreateWorkingHourResult(
    CreateWorkingHourStatus Status,
    WorkingHour? WorkingHour
);