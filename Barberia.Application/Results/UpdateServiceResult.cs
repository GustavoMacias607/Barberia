using Barberia.Application.Enums;
using Barberia.Domain.Entities;

namespace Barberia.Application.Results;

public record UpdateServiceResult(
    UpdateServiceStatus status,
    Service? service
);
