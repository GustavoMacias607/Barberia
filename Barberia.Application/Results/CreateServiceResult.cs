using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;

public record CreateServiceResult(
    CreateServiceStatus status,
    Service? service
);
