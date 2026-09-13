using System.ComponentModel.DataAnnotations;

namespace Barberia.Application.DTOs.Services;

public class CreateServiceRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
}
