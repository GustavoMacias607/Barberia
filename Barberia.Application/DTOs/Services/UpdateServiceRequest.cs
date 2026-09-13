using System.ComponentModel.DataAnnotations;

namespace Barberia.Application.DTOs.Services;

public class UpdateServiceRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
}
