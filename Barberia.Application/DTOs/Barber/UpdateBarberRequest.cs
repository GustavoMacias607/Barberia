using System.ComponentModel.DataAnnotations;

namespace Barberia.Application.DTOs.Barber;

public class UpdateBarberRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
}

